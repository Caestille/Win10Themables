﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LiveChartsCore.Defaults;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Windows.Media;
using System.Windows.Input;

namespace ModernThemables.Controls
{
	public class LineSeries<TModel>
	{
		public event EventHandler<PropertyChangedEventArgs> PropertyChanged;
		public Func<IEnumerable<TModel>, TModel, string> TooltipLabelFormatter { get; set; }
		public Brush Stroke { get; set; }
		public Brush Fill { get; set; }

		private IEnumerable<TModel> values;
		public IEnumerable<TModel> Values 
		{
			get => values;
			set
			{
				values = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Values)));
			}
		}
	}

	/// <summary>
	/// Interaction logic for WpfChart.xaml
	/// </summary>
	public partial class WpfChart : UserControl
	{
		private struct ValueWithHeight
		{
			public string Value { get; set; }
			public double Height { get; set; }

			public ValueWithHeight(string value, double height)
			{
				Value = value;
				Height = height;
			}
		}

		private class ChartPoint
		{
			public double X { get; set; }
			public double Y { get; set; }

			public ChartPoint(double x, double y)
			{
				X = x;
				Y = y;
			}
		}

		private class WpfChartSeries
		{
			public IEnumerable<ChartPoint> Data;
			public string PathData { get; }
			public Brush Stroke { get; set; }
			public Brush Fill { get; set; }

			public WpfChartSeries(IEnumerable<ChartPoint> data, Brush stroke, Brush fill)
			{
				Data = data;
				Stroke = stroke;
				Fill = fill;

				PathData = string.Empty;
				bool setM = true;
				foreach (var point in data)
				{
					var pointType = setM ? "M" : "L";
					setM = false;
					PathData += $" {pointType}{point.X} {point.Y}";
				}
			}
		}

		private double plotAreaHeight => DrawableChartSectionBorder.ActualHeight;
		private double plotAreaWidth => DrawableChartSectionBorder.ActualWidth;
		private DateTime timeLastUpdated;
		private TimeSpan updateLimit = TimeSpan.FromMilliseconds(1000 / 60d);
		private bool hasSetSeries;
		private List<LineSeries<DateTimePoint>> subscribedSeries = new();

		public ObservableCollection<LineSeries<DateTimePoint>> Series
		{
			get { return (ObservableCollection<LineSeries<DateTimePoint>>)GetValue(SeriesProperty); }
			set { SetValue(SeriesProperty, value); }
		}
		public static readonly DependencyProperty SeriesProperty = DependencyProperty.Register(
		  "Series", typeof(ObservableCollection<LineSeries<DateTimePoint>>), typeof(WpfChart), new FrameworkPropertyMetadata(null, OnSeriesSet));

		public Func<IEnumerable<DateTimePoint>, DateTimePoint, string> TooltipLabelFormatter
		{
			get { return (Func<IEnumerable<DateTimePoint>, DateTimePoint, string>)GetValue(TooltipLabelFormatterProperty); }
			set { SetValue(TooltipLabelFormatterProperty, value); }
		}
		public static readonly DependencyProperty TooltipLabelFormatterProperty = DependencyProperty.Register(
		  "TooltipLabelFormatter", typeof(Func<IEnumerable<DateTimePoint>, DateTimePoint, string>), typeof(WpfChart), new PropertyMetadata(null));

		public Func<DateTime, string> XAxisFormatter
		{
			get { return (Func<DateTime, string>)GetValue(XAxisFormatterProperty); }
			set { SetValue(XAxisFormatterProperty, value); }
		}
		public static readonly DependencyProperty XAxisFormatterProperty = DependencyProperty.Register(
		  "XAxisFormatter", typeof(Func<DateTime, string>), typeof(WpfChart), new PropertyMetadata(null));

		public Func<DateTime, string> XAxisCursorLabelFormatter
		{
			get { return (Func<DateTime, string>)GetValue(XAxisCursorLabelFormatterProperty); }
			set { SetValue(XAxisCursorLabelFormatterProperty, value); }
		}
		public static readonly DependencyProperty XAxisCursorLabelFormatterProperty = DependencyProperty.Register(
		  "XAxisCursorLabelFormatter", typeof(Func<DateTime, string>), typeof(WpfChart), new PropertyMetadata(null));

		public Func<double, string> YAxisFormatter
		{
			get { return (Func<double, string>)GetValue(YAxisFormatterProperty); }
			set { SetValue(YAxisFormatterProperty, value); }
		}
		public static readonly DependencyProperty YAxisFormatterProperty = DependencyProperty.Register(
		  "YAxisFormatter", typeof(Func<double, string>), typeof(WpfChart), new PropertyMetadata(null));

		public Func<double, string> YAxisCursorLabelFormatter
		{
			get { return (Func<double, string>)GetValue(YAxisCursorLabelFormatterProperty); }
			set { SetValue(YAxisCursorLabelFormatterProperty, value); }
		}
		public static readonly DependencyProperty YAxisCursorLabelFormatterProperty = DependencyProperty.Register(
		  "YAxisCursorLabelFormatter", typeof(Func<double, string>), typeof(WpfChart), new PropertyMetadata(null));

		public bool ShowXSeparatorLines
		{
			get { return (bool)GetValue(ShowXSeparatorLinesProperty); }
			set { SetValue(ShowXSeparatorLinesProperty, value); }
		}
		public static readonly DependencyProperty ShowXSeparatorLinesProperty = DependencyProperty.Register(
		  "ShowXSeparatorLines", typeof(bool), typeof(WpfChart), new PropertyMetadata(true));

		public bool ShowYSeparatorLines
		{
			get { return (bool)GetValue(ShowYSeparatorLinesProperty); }
			set { SetValue(ShowYSeparatorLinesProperty, value); }
		}
		public static readonly DependencyProperty ShowYSeparatorLinesProperty = DependencyProperty.Register(
		  "ShowYSeparatorLines", typeof(bool), typeof(WpfChart), new PropertyMetadata(true));

		private ObservableCollection<WpfChartSeries> ConvertedSeries
		{
			get { return (ObservableCollection<WpfChartSeries>)GetValue(ConvertedSeriesProperty); }
			set { SetValue(ConvertedSeriesProperty, value); }
		}
		public static readonly DependencyProperty ConvertedSeriesProperty = DependencyProperty.Register(
		  "ConvertedSeries", typeof(ObservableCollection<WpfChartSeries>), typeof(WpfChart), new PropertyMetadata(new ObservableCollection<WpfChartSeries>()));

		private ObservableCollection<string> XAxisLabels
		{
			get { return (ObservableCollection<string>)GetValue(XAxisLabelsProperty); }
			set { SetValue(XAxisLabelsProperty, value); }
		}
		public static readonly DependencyProperty XAxisLabelsProperty = DependencyProperty.Register(
		  "XAxisLabels", typeof(ObservableCollection<string>), typeof(WpfChart), new PropertyMetadata(new ObservableCollection<string>()));

		private ObservableCollection<ValueWithHeight> YAxisLabels
		{
			get { return (ObservableCollection<ValueWithHeight>)GetValue(YAxisLabelsProperty); }
			set { SetValue(YAxisLabelsProperty, value); }
		}
		public static readonly DependencyProperty YAxisLabelsProperty = DependencyProperty.Register(
		  "YAxisLabels", typeof(ObservableCollection<ValueWithHeight>), typeof(WpfChart), new PropertyMetadata(new ObservableCollection<ValueWithHeight>()));

		private double XAxisLabelsWidth
		{
			get { return (double)GetValue(XAxisLabelsWidthProperty); }
			set { SetValue(XAxisLabelsWidthProperty, value); }
		}
		public static readonly DependencyProperty XAxisLabelsWidthProperty = DependencyProperty.Register(
		  "XAxisLabelsWidth", typeof(double), typeof(WpfChart), new PropertyMetadata(0d));

		public WpfChart()
		{
			InitializeComponent();
		}

		private static void OnSeriesSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is not WpfChart chart) return;

			if (chart.Series == null)
			{
				foreach (var series in chart.subscribedSeries)
				{
					series.PropertyChanged -= chart.Series_PropertyChanged;
				}

				return;
			}

			chart.Subscribe(chart.Series);
			chart.hasSetSeries = true;
		}

		private void Subscribe(ObservableCollection<LineSeries<DateTimePoint>> series)
		{
			series.CollectionChanged += Series_CollectionChanged;
			if (!hasSetSeries)
			{
				foreach (LineSeries<DateTimePoint> item in series)
				{
					item.PropertyChanged += Series_PropertyChanged;
					subscribedSeries.Add(item);
				}
			}
			Series_PropertyChanged(this, new PropertyChangedEventArgs(nameof(Series)));
		}

		private void Series_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			foreach (LineSeries<DateTimePoint> series in e.OldItems)
			{
				series.PropertyChanged -= Series_PropertyChanged;
				subscribedSeries.Add(series);
			}

			foreach (LineSeries<DateTimePoint> series in e.NewItems)
			{
				series.PropertyChanged += Series_PropertyChanged;
				subscribedSeries.Remove(series);
			}
		}

		private void Series_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			RenderChart(false);
		}

		private void RenderChart(bool isResize)
		{
			Application.Current.Dispatcher.BeginInvoke(() =>
			{
				if (Series is null || !Series.Any()) return;

				(var xMin, var yMin, var xMax, var yMax, var xRange, var yRange) = GetAxisValues(Series.First());

				SetXAxisLabels(xMin, xRange, isResize);
				SetYAxisLabels(yMin, yMax, yRange, isResize);

				var points = GetPointsForSeries(xMin, xRange, yMin, yRange, Series.First());

				ConvertedSeries = new ObservableCollection<WpfChartSeries>()
					{ new WpfChartSeries(points, Series.First().Stroke, Series.First().Fill), };
			});
		}

		private (DateTime xMin, double yMin, DateTime xMax, double yMax, TimeSpan xRange, double yRange) 
			GetAxisValues(LineSeries<DateTimePoint> series)
		{
			var xMin = series.Values.Min(x => x.DateTime);
			var xMax = series.Values.Max(x => x.DateTime);
			var yMin = series.Values.Min(x => x.Value).Value;
			var yMax = series.Values.Max(x => x.Value).Value;

			var xRange = xMax - xMin;
			var yRange = yMax - yMin;
			yMin -= yRange * 0.1;
			yMax += yRange * 0.1;
			yRange = yMax - yMin;

			return (xMin, yMin, xMax, yMax, xRange, yRange);
		}

		private void SetXAxisLabels(DateTime xMin, TimeSpan xRange, bool isResize)
		{
			var xAxisItemCount = Math.Floor(plotAreaWidth / 60);
			if (!isResize)
			{
				var xLabelStep = xRange / xAxisItemCount;
				XAxisLabels.Clear();

				DateTime currentXStep = xMin;
				for (int i = 0; i < xAxisItemCount; i++)
				{
					currentXStep += i == 0 || i == xAxisItemCount ? xLabelStep / 2 : xLabelStep;
					XAxisLabels.Add(XAxisFormatter == null ? currentXStep.ToString() : XAxisFormatter(currentXStep));
				}
			}
			
			XAxisLabelsWidth = plotAreaWidth / xAxisItemCount;
		}

		private void SetYAxisLabels(double yMin, double yMax, double yRange, bool isResize)
		{
			var yAxisItemsCount = Math.Floor(plotAreaHeight / 50);
			var yLabelStep = yRange / yAxisItemsCount;
			var labels = GetYSteps(yRange, yAxisItemsCount, yMin, yMax).ToList();
			var labels2 = labels.Select(y => new ValueWithHeight()
			{
				Value = YAxisFormatter == null ? Math.Round(y, 2).ToString() : YAxisFormatter(y),
				Height = ((y - yMin) / yRange * plotAreaHeight) - (labels.ToList().IndexOf(y) > 0
					? (labels[labels.ToList().IndexOf(y) - 1] - yMin) / yRange * plotAreaHeight
					: 0),
			});
			YAxisLabels = new ObservableCollection<ValueWithHeight>(labels2.Reverse());
		}

		private List<ChartPoint> GetPointsForSeries(
			DateTime xMin, TimeSpan xRange, double yMin, double yRange, LineSeries<DateTimePoint> series)
		{
			List<ChartPoint> points = new();
			foreach (var point in series.Values.OrderBy(x => x.DateTime))
			{
				double x = (double)(point.DateTime - xMin).Ticks / (double)xRange.Ticks * (double)plotAreaWidth;
				double y = plotAreaHeight - (point.Value.Value - yMin) / yRange * plotAreaHeight;
				points.Add(new ChartPoint(x, y));
			}
			return points;
		}

		private List<double> GetYSteps(double yRange, double yAxisItemsCount, double yMin, double yMax)
		{
			var idealStep = yRange / yAxisItemsCount;
			double min = double.MaxValue;
			int stepAtMin = 1;
			var roundedSteps 
				= new List<int>() 
					{ 1, 10, 100, 500, 1000, 1500, 2000, 3000, 4000, 5000, 10000, 20000, 50000, 1000000, 10000000 };
			roundedSteps.Reverse();
			foreach (var step in roundedSteps)
			{
				var val = Math.Abs(idealStep - step);
				if (val < min)
				{
					min = val;
					stepAtMin = step;
				}
			}

			List<double> yVals = new();
			bool startAt0 = yMin <= 0 && yMax >= 0;
			if (startAt0)
			{
				double currVal = 0;
				while (currVal > yMin)
				{
					yVals.Insert(0, currVal);
					currVal -= stepAtMin;
				}

				currVal = stepAtMin;

				while (currVal < yMax)
				{
					yVals.Add(currVal);
					currVal += stepAtMin;
				}
			}
			else
			{
				int dir = yMax < 0 ? -1 : 1;
				double currVal = 0;
				bool adding = true;
				bool hasStartedAdding = false;
				while (adding)
				{
					if (currVal < yMax && currVal > yMin)
					{
						hasStartedAdding = true;
						yVals.Add(currVal);
					}
					else if (hasStartedAdding)
					{
						adding = false;
					}

					currVal += dir * stepAtMin;
				}
			}

			return yVals;
		}

		private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			Series_PropertyChanged(this, new PropertyChangedEventArgs(nameof(Series)));
		}

		private void DrawableChartSectionBorder_MouseMove(object sender, MouseEventArgs e)
		{
			if (DateTime.Now - timeLastUpdated > updateLimit)
			{
				timeLastUpdated = DateTime.Now;
				var mouseLoc = e.GetPosition(Grid);
				XCrosshair.Margin = new Thickness(0, mouseLoc.Y, 0, 0);
				YCrosshair.Margin = new Thickness(mouseLoc.X, 0, 0, 0);
				XCrosshairValueDisplay.Margin = new Thickness(mouseLoc.X - 50, 0, 0, -XAxisRow.ActualHeight);
				YCrosshairValueDisplay.Margin = new Thickness(-YAxisColumn.ActualWidth, mouseLoc.Y - 10, 0, 0);

				(var xMin, var yMin, var xMax, var yMax, var xRange, var yRange) = GetAxisValues(Series.First());
				var xPercent = mouseLoc.X / plotAreaWidth;
				var yPercent = mouseLoc.Y / plotAreaHeight;

				var xVal = xMin.Add(xPercent * xRange);
				var yVal = ((1 - yPercent) * yRange + yMin);

				XCrosshairValueLabel.Text = XAxisCursorLabelFormatter == null ? xVal.ToString() : XAxisCursorLabelFormatter(xVal);
				YCrosshairValueLabel.Text = YAxisCursorLabelFormatter == null ? Math.Round(yVal, 2).ToString() : YAxisCursorLabelFormatter(yVal);
			}
		}
	}
}
