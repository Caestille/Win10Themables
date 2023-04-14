﻿using ModernThemables.Charting.Interfaces;
using ModernThemables.Charting.Models;
using ModernThemables.Charting.ViewModels.PieChart;
using System.Collections.ObjectModel;
using System.Windows;

namespace ModernThemables.Charting.Controls
{
    public partial class PieChart // .DependencyProperties
	{
		#region Public properties

		public ObservableCollection<ISeries> Series
		{
			get => (ObservableCollection<ISeries>)GetValue(SeriesProperty);
			set => SetValue(SeriesProperty, value);
		}
		public static readonly DependencyProperty SeriesProperty = DependencyProperty.Register(
			"Series",
			typeof(ObservableCollection<ISeries>),
			typeof(PieChart),
			new FrameworkPropertyMetadata(null, OnSeriesSet));

		public double InnerRadiusFraction
		{
			get => (double)GetValue(InnerRadiusFractionProperty);
			set => SetValue(InnerRadiusFractionProperty, value);
		}
		public static readonly DependencyProperty InnerRadiusFractionProperty = DependencyProperty.Register(
			"InnerRadiusFraction",
			typeof(double),
			typeof(PieChart),
			new PropertyMetadata(0d));

		public double LabelRadiusFraction
		{
			get => (double)GetValue(LabelRadiusFractionProperty);
			set => SetValue(LabelRadiusFractionProperty, value);
		}
		public static readonly DependencyProperty LabelRadiusFractionProperty = DependencyProperty.Register(
			"LabelRadiusFraction",
			typeof(double),
			typeof(PieChart),
			new PropertyMetadata(0d));

		public DataTemplate TooltipTemplate
		{
			get => (DataTemplate)GetValue(TooltipTemplateProperty);
			set => SetValue(TooltipTemplateProperty, value);
		}
		public static readonly DependencyProperty TooltipTemplateProperty = DependencyProperty.Register(
			"TooltipTemplate",
			typeof(DataTemplate),
			typeof(PieChart),
			new PropertyMetadata(null));

		public DataTemplate LegendTemplate
		{
			get => (DataTemplate)GetValue(LegendTemplateProperty);
			set => SetValue(LegendTemplateProperty, value);
		}
		public static readonly DependencyProperty LegendTemplateProperty = DependencyProperty.Register(
			"LegendTemplate",
			typeof(DataTemplate),
			typeof(PieChart),
			new PropertyMetadata(null));

		public LegendLocation LegendLocation
		{
			get => (LegendLocation)GetValue(LegendLocationProperty);
			set => SetValue(LegendLocationProperty, value);
		}
		public static readonly DependencyProperty LegendLocationProperty = DependencyProperty.Register(
			"LegendLocation",
			typeof(LegendLocation),
			typeof(PieChart),
			new UIPropertyMetadata(LegendLocation.None, OnLegendLocationSet));

		public TooltipLocation TooltipLocation
		{
			get => (TooltipLocation)GetValue(TooltipLocationProperty);
			set => SetValue(TooltipLocationProperty, value);
		}
		public static readonly DependencyProperty TooltipLocationProperty = DependencyProperty.Register(
			"TooltipLocation",
			typeof(TooltipLocation),
			typeof(PieChart),
			new FrameworkPropertyMetadata(TooltipLocation.Cursor));

		public double TooltipOpacity
		{
			get => (double)GetValue(TooltipOpacityProperty);
			set => SetValue(TooltipOpacityProperty, value);
		}
		public static readonly DependencyProperty TooltipOpacityProperty = DependencyProperty.Register(
			"TooltipOpacity",
			typeof(double),
			typeof(PieChart),
			new PropertyMetadata(1d));

		#endregion

		#region Private properties

		private ObservableCollection<InternalPieSeriesViewModel> InternalSeries
		{
			get => (ObservableCollection<InternalPieSeriesViewModel>)GetValue(InternalSeriesProperty);
			set => SetValue(InternalSeriesProperty, value);
		}
		public static readonly DependencyProperty InternalSeriesProperty = DependencyProperty.Register(
			"InternalSeries",
			typeof(ObservableCollection<InternalPieSeriesViewModel>),
			typeof(PieChart),
			new PropertyMetadata(new ObservableCollection<InternalPieSeriesViewModel>()));

		private bool IsTooltipVisible
		{
			get => (bool)GetValue(IsTooltipVisibleProperty);
			set => SetValue(IsTooltipVisibleProperty, value);
		}
		public static readonly DependencyProperty IsTooltipVisibleProperty = DependencyProperty.Register(
			"IsTooltipVisible",
			typeof(bool),
			typeof(PieChart),
			new PropertyMetadata(true));

		private InternalPieWedgeViewModel TooltipWedge
		{
			get => (InternalPieWedgeViewModel)GetValue(TooltipWedgeProperty);
			set => SetValue(TooltipWedgeProperty, value);
		}
		public static readonly DependencyProperty TooltipWedgeProperty = DependencyProperty.Register(
			"TooltipWedge",
			typeof(InternalPieWedgeViewModel),
			typeof(PieChart),
			new PropertyMetadata(null));

		private string TooltipString
		{
			get => (string)GetValue(TooltipStringProperty);
			set => SetValue(TooltipStringProperty, value);
		}
		public static readonly DependencyProperty TooltipStringProperty = DependencyProperty.Register(
			"TooltipString",
			typeof(string),
			typeof(PieChart),
			new PropertyMetadata(""));

		private bool HasData
		{
			get => (bool)GetValue(HasDataProperty);
			set => SetValue(HasDataProperty, value);
		}
		public static readonly DependencyProperty HasDataProperty = DependencyProperty.Register(
			"HasData",
			typeof(bool),
			typeof(PieChart),
			new PropertyMetadata(false));

		#endregion
	}
}