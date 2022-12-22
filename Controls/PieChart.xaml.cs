﻿using CoreUtilities.HelperClasses.Extensions;
using CoreUtilities.Services;
using ModernThemables.HelperClasses.Charting.Brushes;
using ModernThemables.HelperClasses.Charting;
using ModernThemables.HelperClasses.Charting.PieChart;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ModernThemables.ViewModels.Charting.CartesianChart;
using ModernThemables.ViewModels.Charting.PieChart;
using ModernThemables.Converters;
using System.Reflection.Metadata;
using System.Diagnostics;
using System.Xml.Linq;

namespace ModernThemables.Controls
{
	/// <summary>
	/// Interaction logic for WpfChart.xaml
	/// </summary>
	public partial class PieChart : UserControl
	{
		private DateTime timeLastUpdated;
		private TimeSpan updateLimit = TimeSpan.FromMilliseconds(1000 / 60d);
		private List<PieSeries> subscribedSeries = new();

		private KeepAliveTriggerService resizeTrigger;

		private Point? lastMouseMovePoint;
		private bool ignoreNextMouseMove;

		private bool tooltipLeft;
		private bool tooltipTop = true;

		private bool preventTrigger;
		
		private IEnumerable<InternalPieWedge> allWedges
			=> this.InternalSeries.Aggregate(new List<InternalPieWedge>(), (list, series) => { list.AddRange(series.Wedges); return list; });

		private BlockingCollection<Action> renderQueue;
		private bool renderInProgress;

		private Thread renderThread;
		private bool runRenderThread = true;

		public PieChart()
		{
			InitializeComponent();
			this.Loaded += PieChart_Loaded;

			renderQueue = new BlockingCollection<Action>();
			renderThread = new Thread(new ThreadStart(() =>
			{
				while (runRenderThread)
				{
					while (renderInProgress)
					{
						Thread.Sleep(1);
					}
					if (renderQueue.Any())
						renderQueue.Take().Invoke();
					Thread.Sleep(1);
				}
			}));
			renderThread.Start();

			NameScope.SetNameScope(ContextMenu, NameScope.GetNameScope(this));

			resizeTrigger = new KeepAliveTriggerService(QueueRenderChart, 100);
		}

		private static void OnTooltipLocationSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not PieChart chart) return;

			chart.IsTooltipByCursor = chart.TooltipLocation == TooltipLocation.Cursor;
		}

		private static async void OnLegendLocationSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not PieChart chart) return;

			switch (chart.LegendLocation)
			{
				case LegendLocation.Left:
					chart.LegendGrid.SetValue(Grid.RowProperty, 1);
					chart.LegendGrid.SetValue(Grid.ColumnProperty, 0);
					chart.LegendGrid.Visibility = Visibility.Visible;
					chart.LegendGrid.Margin = new Thickness(0, 10, 15, 0);
					chart.LegendItemsControl.ItemsPanel = (ItemsPanelTemplate)chart.Resources["StackTemplate"];
					break;
				case LegendLocation.Top:
					chart.LegendGrid.SetValue(Grid.RowProperty, 0);
					chart.LegendGrid.SetValue(Grid.ColumnProperty, 1);
					chart.LegendGrid.Visibility = Visibility.Visible;
					chart.LegendGrid.Margin = new Thickness(0, 0, 0, 15);
					chart.LegendItemsControl.ItemsPanel = (ItemsPanelTemplate)chart.Resources["WrapTemplate"];
					break;
				case LegendLocation.Right:
					chart.LegendGrid.SetValue(Grid.RowProperty, 1);
					chart.LegendGrid.SetValue(Grid.ColumnProperty, 2);
					chart.LegendGrid.Visibility = Visibility.Visible;
					chart.LegendGrid.Margin = new Thickness(15, 10, 0, 0);
					chart.LegendItemsControl.ItemsPanel = (ItemsPanelTemplate)chart.Resources["StackTemplate"];
					break;
				case LegendLocation.Bottom:
					chart.LegendGrid.SetValue(Grid.RowProperty, 2);
					chart.LegendGrid.SetValue(Grid.ColumnProperty, 1);
					chart.LegendGrid.Visibility = Visibility.Visible;
					chart.LegendGrid.Margin = new Thickness(0, 15, 0, 0);
					chart.LegendItemsControl.ItemsPanel = (ItemsPanelTemplate)chart.Resources["WrapTemplate"];
					break;
				case LegendLocation.None:
					chart.LegendGrid.Visibility = Visibility.Collapsed;
					break;
			}

			await Task.Delay(1);
			chart.QueueRenderChart();
		}

		#region Subscribe to series'

		private static void OnSeriesSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not PieChart chart) return;

			foreach (var series in chart.subscribedSeries)
			{
				series.PropertyChanged -= chart.Series_PropertyChanged;
				foreach (var wedge in series.Wedges) wedge.FocusedChanged -= chart.Wedge_FocusedChanged;
			}

			chart.subscribedSeries.Clear();

			chart.Subscribe(chart.Series);

			if (!chart.Series.Any() || !chart.Series.Any()) return;

			chart.QueueRenderChart();
		}

		private void Subscribe(ObservableCollection<PieSeries> series)
		{
			series.CollectionChanged += Series_CollectionChanged;
			foreach (PieSeries item in series)
			{
				item.PropertyChanged += Series_PropertyChanged;
				foreach (var wedge in item.Wedges) wedge.FocusedChanged += Wedge_FocusedChanged;
				subscribedSeries.Add(item);
			}
		}

		private async void Series_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				QueueRenderChart();
				return;
			}

			var oldItems = new List<PieSeries>();
			if ((e.Action == NotifyCollectionChangedAction.Replace || e.Action == NotifyCollectionChangedAction.Remove)
				&& e.OldItems != null)
			{
				foreach (PieSeries series in e.OldItems)
				{
					series.PropertyChanged -= Series_PropertyChanged;
					foreach (var wedge in series.Wedges) wedge.FocusedChanged -= Wedge_FocusedChanged;
					oldItems.Add(series);
					subscribedSeries.Remove(series);
				}
			}

			var newItems = new List<PieSeries>();
			if ((e.Action == NotifyCollectionChangedAction.Replace || e.Action == NotifyCollectionChangedAction.Add)
				&& e.NewItems != null)
			{
				foreach (PieSeries series in e.NewItems)
				{
					series.PropertyChanged += Series_PropertyChanged;
					foreach (var wedge in series.Wedges) wedge.FocusedChanged += Wedge_FocusedChanged;
					newItems.Add(series);
					subscribedSeries.Add(series);
				}
			}

			QueueRenderChart();
		}

		private async void Series_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (sender is PieSeries)
			{
				QueueRenderChart();
			}
		}

		#endregion

		private void QueueRenderChart()
		{
			renderQueue.Add(RenderChart);
		}

		private void RenderChart()
		{
			Application.Current.Dispatcher.Invoke(async () =>
			{
				renderInProgress = true;

				var newSeries = new List<InternalPieSeriesViewModel>();

				foreach (var series in Series)
				{
					var wedges = await GetWedgesForSeries(series);

					newSeries.Add(new InternalPieSeriesViewModel(
						series.Name,
						new ObservableCollection<InternalPieWedge>(wedges),
						series.TooltipLabelFormatter));

					if (!series.Wedges.Any()) continue;
				}

				InternalSeries = new ObservableCollection<InternalPieSeriesViewModel>(newSeries);

				foreach (var series in InternalSeries)
				{
					foreach (var wedge in series.Wedges)
					{
						wedge.ResizeTrigger = !wedge.ResizeTrigger;
					}
				}

				renderInProgress = false;
			});
		}

		#region Calculations		

		private async Task<List<InternalPieWedge>> GetWedgesForSeries(PieSeries? series)
		{
			var convertedSeries = new List<InternalPieWedge>();

			if (series == null) return convertedSeries;

			var sum = series.Wedges.Sum(x => x.Value);
			var angleSum = 0d;

			foreach (var wedge in series.Wedges)
			{
				InternalPieWedge? matchingWedge = null;

				convertedSeries.Add(new InternalPieWedge(
					wedge.Name,
					wedge.Identifier,
					wedge.Value / sum * 100,
					angleSum / sum * 360,
					matchingWedge != null
						? matchingWedge.Stroke
						: wedge.Stroke ?? new SolidBrush(ColorExtensions.RandomColour(50)),
					matchingWedge != null
						? matchingWedge.Fill
						: wedge.Fill ?? new SolidBrush(ColorExtensions.RandomColour(50))));

				angleSum += wedge.Value;
			}

			return convertedSeries;
		}

		#endregion

		#region Grid events

		private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			//resizeTrigger.Refresh();
		}

		#endregion

		#region Mouse events

		private void MouseCaptureGrid_MouseMove(object sender, MouseEventArgs e)
		{
			var converter = new PieCentreRadiusConverter();
			var centreX = (double)converter.Convert(
				new object[] { SeriesItemsControl.ActualWidth, SeriesItemsControl.ActualHeight },
				null, "CentreX", null);
			var centreY = (double)converter.Convert(
				new object[] { SeriesItemsControl.ActualWidth, SeriesItemsControl.ActualHeight },
				null, "CentreY", null);
			var radius = (double)converter.Convert(
				new object[] { SeriesItemsControl.ActualWidth, SeriesItemsControl.ActualHeight },
				null, "Radius", null);

			var mouseLoc = e.GetPosition(SeriesItemsControl);
			mouseLoc = new Point(mouseLoc.X -= SeriesItemsControl.ActualWidth / 2 - radius / 0.9, mouseLoc.Y);

			var hypLength = Math.Sqrt(
				Math.Pow(Math.Abs(mouseLoc.X - centreX), 2)
				+ Math.Pow(Math.Abs(mouseLoc.Y - centreY), 2));

			if (hypLength > radius)
			{
				foreach (var wedge in allWedges) wedge.IsMouseOver = false;
				return;
			}

			var angle = GetMouseAngleFromPoint(mouseLoc, new Point(centreX, centreY));

			foreach (var series in InternalSeries)
			{
				foreach (var wedge in series.Wedges)
				{
					if (angle > wedge.StartAngle
						&& angle < wedge.StartAngle + wedge.Percent * 360d /100d)
					{
						wedge.IsMouseOver = true;
					}
					else if (wedge.IsMouseOver)
					{
						wedge.IsMouseOver = false;
					}
				}
			}
		}

		private void MouseCaptureGrid_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			
		}

		#endregion

		private bool HasGotData()
		{
			HasData = Series != null && Series.Any();
			return HasData;
		}

		private double GetMouseAngleFromPoint(Point mouseLoc, Point point)
		{
			var hypLength = Math.Sqrt(
				Math.Pow(Math.Abs(mouseLoc.X - point.X), 2)
				+ Math.Pow(Math.Abs(mouseLoc.Y - point.Y), 2));

			var oppLength = Math.Abs(mouseLoc.X - point.X);
			var angle = Math.Asin(oppLength / hypLength) * 180 / Math.PI;

			if (mouseLoc.X <= point.X && mouseLoc.Y <= point.Y)
			{
				angle = 360 - angle;
			}

			if (mouseLoc.Y > point.Y)
			{
				if (mouseLoc.X <= point.X)
				{
					angle = 180 + angle;
				}
				else
				{
					angle = 180 - angle;
				}
			}

			return angle;
		}

		private void YAxisItemsControl_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			ignoreNextMouseMove = true;
		}

		private void Wedge_FocusedChanged(object? sender, bool e)
		{
			if (sender is not PieWedge wedge) return;
			var id = wedge.Identifier;
			var matchingInternalWedge = InternalSeries.First(x => x.Wedges.Any(y => y.Identifier == id)).Wedges.First(x => x.Identifier == id);
			matchingInternalWedge.IsMouseOver = e;
		}

		private void PieChart_Loaded(object sender, RoutedEventArgs e)
		{
			this.Loaded -= PieChart_Loaded;
			Application.Current.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
			OnLegendLocationSet(this, new DependencyPropertyChangedEventArgs());
		}

		private void Dispatcher_ShutdownStarted(object? sender, EventArgs e)
		{
			resizeTrigger.Stop();
			runRenderThread = false;
			foreach (var series in subscribedSeries)
			{
				series.PropertyChanged -= Series_PropertyChanged;
			}
		}
	}
}