﻿using System;
using System.Windows;

namespace ModernThemables.Controls
{
	public partial class RangeSlider
	{
		private const int borderPadding = 11;

		public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(0d));
		public double Minimum
		{
			get => (double)GetValue(MinimumProperty);
			set
			{
				SetValue(MinimumProperty, value);
				AdjustSliderRangeBorderValues();
			}
		}

		public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(1d));
		public double Maximum
		{
			get => (double)GetValue(MaximumProperty);
			set
			{
				SetValue(MaximumProperty, value);
				AdjustSliderRangeBorderValues();
			}
		}

		public static readonly DependencyProperty SliderSeparationProperty = DependencyProperty.Register("SliderSeparation", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(1d));
		public double SliderSeparation
		{
			get => (double)GetValue(SliderSeparationProperty);
			set 
			{ 
				SetValue(SliderSeparationProperty, value);
				AdjustSliderRangeBorderValues();
			}
		}

		public static readonly DependencyProperty LowerValueProperty = DependencyProperty.Register("LowerValue", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(0d));
		public double LowerValue
		{
			get => (double)GetValue(LowerValueProperty);
			set
			{
				if (value > Maximum - SliderSeparation)
				{
					value = Maximum - SliderSeparation;
				}
				double oldValue = LowerValue;
				SetValue(LowerValueProperty, value);
				LowerValueChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<double>(oldValue, value));
				if (value > UpperValue - SliderSeparation)
				{
					UpperValue = Math.Min(Maximum, LowerValue + SliderSeparation);
				}
				AdjustSliderRangeBorderValues();
			}
		}

		public static readonly DependencyProperty UpperValueProperty = DependencyProperty.Register("UpperValue", typeof(double), typeof(RangeSlider), new UIPropertyMetadata(0d));
		public double UpperValue
		{
			get => (double)GetValue(UpperValueProperty);
			set
			{
				if (value < Minimum + SliderSeparation)
				{
					value = Minimum + SliderSeparation;
				}
				double oldValue = UpperValue;
				SetValue(UpperValueProperty, value);
				UpperValueChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<double>(oldValue, value));
				if (value < LowerValue + SliderSeparation)
				{
					LowerValue = Math.Max(Minimum, UpperValue - SliderSeparation);
				}
				AdjustSliderRangeBorderValues();
			}
		}

		public event EventHandler<RoutedPropertyChangedEventArgs<double>>? LowerValueChanged;
		public event EventHandler<RoutedPropertyChangedEventArgs<double>>? UpperValueChanged;

		public RangeSlider()
		{
			InitializeComponent();

			UpperSlider.ValueChanged += UpperSlider_ValueChanged;
			LowerSlider.ValueChanged += LowerSlider_ValueChanged;
			this.Loaded += CustomSlider_Loaded;
		}

		private void CustomSlider_Loaded(object sender, RoutedEventArgs e)
		{
			AdjustSliderRangeBorderValues();
			this.Loaded -= CustomSlider_Loaded;
		}

		private void LowerSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			LowerValue = e.NewValue;
		}

		private void UpperSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			UpperValue = e.NewValue;
		}

		private void AdjustSliderRangeBorderValues()
		{
			double width = BaseGrid.ActualWidth - borderPadding;
			double range = Maximum - Minimum;
			OuterBorder.Margin = range == 0 
				? new Thickness(width,0,10,0) 
				: new Thickness((LowerValue - Minimum) / range * width + 10, 0, width - (UpperValue - Minimum) / range * width + 7, 0);
		}

		private void LowerSlider_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			AdjustSliderRangeBorderValues();
		}
	}
}