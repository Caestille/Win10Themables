﻿using CoreUtilities.HelperClasses.Extensions;
using CoreUtilities.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using ModernThemables.Charting.ViewModels;
using ModernThemables.Charting.Models;
using ModernThemables.Charting.Interfaces;
using ModernThemables.Charting.Services;

namespace ModernThemables.Charting.Controls
{
	/// <summary>
	/// Interaction logic for BarChart.xaml
	/// </summary>
	public partial class BarChart : UserControl
	{
		private double plotAreaHeight => TooltipControl.ActualHeight;
		private double plotAreaWidth => TooltipControl.ActualWidth;

		private KeepAliveTriggerService resizeTrigger;

		private bool isSingleXPoint;

		private BlockingCollection<Action> renderQueue;
		private bool renderInProgress;
		private bool runRenderThread = true;

		private SeriesWatcherService seriesWatcher;

		private bool hasData => Series != null && Series.Any(x => x.Values.Any());

		public BarChart()
		{
			InitializeComponent();
			this.Loaded += WpfChart_Loaded;

			seriesWatcher = new SeriesWatcherService(QueueRenderChart);

			renderQueue = new BlockingCollection<Action>();
			new Thread(new ThreadStart(() =>
			{
				var sleepTimeMs = 16;
				while (runRenderThread)
				{
					while (renderInProgress)
					{
						Thread.Sleep(sleepTimeMs);
					}
					if (renderQueue.Any())
						renderQueue.Take().Invoke();
					Thread.Sleep(sleepTimeMs);
				}
			})).Start();

			TooltipGetterFunc = new Func<Point, IEnumerable<TooltipViewModel>>((point) =>
			{
				var tooltipPoints = new List<TooltipViewModel>();

				foreach (var bar in InternalSeries)
				{
					bar.IsMouseOver = (point.X - bar.X) <= BarWidth && (point.X - bar.X) >= 0;
				}

				var tooltipBar = InternalSeries.FirstOrDefault(x => x.IsMouseOver);

				if (tooltipBar != null)
				{
					var matchingSeries = Series.First(x => x.Values.Any(y => y.Identifier == tooltipBar.Identifier));
					var formattedValue = matchingSeries.ValueFormatter(tooltipBar.BackingPoint);
					var matchingWedge = matchingSeries.Values.First(x => x.Identifier == tooltipBar.Identifier);
					var category = matchingSeries.TooltipLabelFormatter != null
						? matchingSeries.TooltipLabelFormatter(matchingSeries.Values, matchingWedge)
						: matchingSeries.Name;
					var formattedDate = tooltipBar.BackingPoint.Name;

					tooltipPoints.Add(new TooltipViewModel(tooltipBar, tooltipBar.Fill.CoreBrush, formattedValue, formattedDate, category));
				}

				return tooltipPoints;
			});

			resizeTrigger = new KeepAliveTriggerService(() => { QueueRenderChart(null, null, true); }, 100);
		}

		private static void TriggerReRender(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not BarChart chart) return;

			chart.QueueRenderChart(null, null, true);
		}

		private static void OnSeriesSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not BarChart chart) return;

			chart.seriesWatcher.ProvideSeries(chart.Series);
		}

		private static async void OnLegendLocationSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not BarChart chart) return;

			var properties = ChartHelper.GetLegendProperties(chart.LegendLocation);

			chart.LegendGrid.SetValue(Grid.RowProperty, properties.row);
			chart.LegendGrid.SetValue(Grid.ColumnProperty, properties.column);
			chart.LegendGrid.Visibility = properties.visibility;
			chart.LegendGrid.Margin = properties.margin;

			await Task.Delay(1);
			chart.QueueRenderChart(null, null, true);
		}

		private void QueueRenderChart(
			IEnumerable<ISeries>? addedSeries, IEnumerable<ISeries>? removedSeries, bool invalidateAll = false)
		{
			renderQueue.Add(new Action(() => RenderChart(addedSeries, removedSeries, invalidateAll)));
		}

		private void RenderChart(
			IEnumerable<ISeries>? addedSeries, IEnumerable<ISeries>? removedSeries, bool invalidateAll = false)
		{
			Application.Current.Dispatcher.Invoke(async () =>
			{
				renderInProgress = true;
				this.Dispatcher.Invoke(DispatcherPriority.Render, SetYAxisLabels);

				var barSep = BarSeparationPixels;
				var groupSep = BarGroupSeparationPixels;

				var source = Series.Clone().ToList();

				var groups = Series.SelectMany(x => x.Values.Select(y => x.Values.IndexOf(y))).Distinct();
				var groupedBars = groups.Select(
					x => source.Select(
						y => x < y.Values.Count 
							? new Tuple<IChartEntity, IChartBrush, IChartBrush>(y.Values[x], y.Fill, y.Stroke) 
							: null)
					.Where(y => y != null));
				var labels = groupedBars.Select(x => x.First()?.Item1?.Name ?? string.Empty);
				double barCount = groupedBars.Any() ? groupedBars.Max(x => x.Count()) : 0;

				var groupWidth = groups.Any() ? ((double)plotAreaWidth / (double)groups.Count()) - groupSep : 0;
				var barWidth = groupWidth > 0 ? (groupWidth / barCount) - barSep : 0;

				this.Dispatcher.Invoke(DispatcherPriority.Render, delegate () { });

				var collection = await Task.Run(() =>
				{
					var ret = new List<InternalChartEntity>();
					var maxHeight = groupedBars.Any() ? groupedBars.Max(x => x.Max(y => y.Item1.YValue)) : 0;

					var currentX = groupSep / 2;
					foreach (var group in groupedBars.Select(x => x.ToList()))
					{
						for (int i = 0; i < barCount; i++)
						{
							if (i < group.Count())
							{
								var bar = group[i];
								ret.Add(new InternalChartEntity(
									currentX, 
									(bar.Item1.YValue / maxHeight) * plotAreaHeight * (1 - 0.1),
									bar.Item1,
									bar.Item3,
									bar.Item2) { Identifier = bar.Item1.Identifier });
							}
							currentX += (barWidth + barSep);
						}
						currentX += groupSep;
					}

					return ret;
				});

				BarWidth = barWidth;
				GroupWidth = Math.Floor(groups.Any() ? ((double)plotAreaWidth / (double)groups.Count()) : 0);
				var radius = BarWidth * BarCornerRadiusFraction / 2;
				BarCornerRadius = new CornerRadius(0, 0, radius, radius);
				this.isSingleXPoint = collection.Count < 2;

				await Task.Delay(18);
				this.Dispatcher.Invoke(DispatcherPriority.Render, () => { });
				await this.Dispatcher.BeginInvoke(
					DispatcherPriority.Render,
					() => { InternalSeries = new ObservableCollection<InternalChartEntity>(collection); });
				await this.Dispatcher.BeginInvoke(DispatcherPriority.Render, () => SetXAxisLabels(labels));

				renderInProgress = false;
			});
		}

		#region Calculations

		private void SetXAxisLabels(IEnumerable<string> labels)
		{
			if (!hasData) return;

			var labels2 = labels.Select(x => new AxisLabel()
			{
				Value = x.ToString(),
				Location = InternalSeries.First(y => y.BackingPoint.Name == x).X + GroupWidth / 2,
			});
			XAxisLabels = new ObservableCollection<AxisLabel>(labels2);
			if (isSingleXPoint)
			{
				XAxisLabels = new ObservableCollection<AxisLabel>()
				{
					new AxisLabel(labels.First(), plotAreaWidth / 2)
				};
			}
		}

		private async void SetYAxisLabels()
		{
			if (!hasData) return;

			var yMax = Series.Max(x => x.Values.Max(y => y.YValue)) * 1.1;
			var yMin = Series.Min(x => x.Values.Min(y => y.YValue));

			var yRange = yMax - yMin;
			var yAxisItemsCount = (int)Math.Max(1, Math.Floor(plotAreaHeight / 50));
			var labels = (await GetYSteps(yAxisItemsCount, yMin, yMax)).ToList();
			var labels2 = labels.Select(y => new AxisLabel()
			{
				Value = YAxisFormatter == null
					? Math.Round(y, 2).ToString()
					: YAxisFormatter(y),
				Location = ((double)(y - yMin) / (double)yRange) * plotAreaHeight,
			});
			YAxisLabels = new ObservableCollection<AxisLabel>(labels2.Reverse());
		}

		private async Task<List<double>> GetYSteps(int yAxisItemsCount, double yMin, double yMax)
		{
			List<double> yVals = new();

			if (YAxisLabelIdentifier != null)
			{
				var currVal = yMin;
				while (currVal < yMax)
				{
					if (YAxisLabelIdentifier(Series.First().Values.First().YValueToImplementation(currVal)))
					{
						yVals.Add(currVal);
					}
					currVal++;
				}
			}
			else
			{
				await Task.Run(() => yVals = ChartHelper.IdealAxisSteps(yAxisItemsCount, yMin, yMax));
			}

			return yVals;
		}

		#endregion

		private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			resizeTrigger.Refresh();
		}

		private void MouseCaptureGrid_MouseLeave(object sender, MouseEventArgs e)
		{
			foreach (var bar in InternalSeries)
			{
				bar.IsMouseOver = false;
			}
		}

		private void WpfChart_Loaded(object sender, RoutedEventArgs e)
		{
			this.Loaded -= WpfChart_Loaded;
			Application.Current.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
			OnLegendLocationSet(this, new DependencyPropertyChangedEventArgs());
		}

		private void Dispatcher_ShutdownStarted(object? sender, EventArgs e)
		{
			resizeTrigger.Stop();
			runRenderThread = false;
			seriesWatcher.Dispose();
		}
	}
}
