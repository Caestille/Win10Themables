﻿using System;
using System.Windows;
using System.Windows.Input;

namespace Win10Themables.Controls
{
	public partial class CustomSlider
	{
		public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(CustomSlider), new UIPropertyMetadata(0d));
		public double Minimum
		{
			get => (double)GetValue(MinimumProperty);
			set => SetValue(MinimumProperty, value);
		}

		public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(CustomSlider), new UIPropertyMetadata(1d));
		public double Maximum
		{
			get => (double)GetValue(MaximumProperty);
			set => SetValue(MaximumProperty, value);
		}

		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(CustomSlider), new UIPropertyMetadata(0d));
		public double Value
		{
			get => (double)GetValue(ValueProperty);
			set
			{
				double oldValue = Value;
				SetValue(ValueProperty, Math.Min(Math.Max(value, Minimum), Maximum));
				ValueChanged?.Invoke(this, new RoutedPropertyChangedEventArgs<double>(oldValue, value));
			}
		}

		public event EventHandler<RoutedPropertyChangedEventArgs<double>>? ValueChanged;

		public CustomSlider()
		{
			InitializeComponent();

			Slider.ValueChanged += Slider_ValueChanged;
			this.Loaded += CustomSlider_Loaded;
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			if (e.Key == Key.Right || e.Key == Key.Left)
			{
				Value += e.Key == Key.Right ? 0.1 * (Maximum - Minimum) : -0.1 * (Maximum - Minimum);
				e.Handled = true;
			}
			base.OnPreviewKeyDown(e);
		}

		private void CustomSlider_Loaded(object sender, RoutedEventArgs e)
		{
			AdjustSliderRangeBorderValues();
			this.Loaded -= CustomSlider_Loaded;
		}

		private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			AdjustSliderRangeBorderValues();
		}

		private void AdjustSliderRangeBorderValues()
		{
			double width = BaseGrid.ActualWidth - 10;
			double range = Maximum - Minimum;
			double rightPadding = (width - Value / range * width);
			if (double.IsNaN(rightPadding))
				rightPadding = 0;
			InnerBorder.Margin = new Thickness(5, 0, rightPadding + 5, 0);
		}
	}
}