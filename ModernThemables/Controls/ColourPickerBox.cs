﻿using ModernThemables.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernThemables.Controls
{
    public class ColourPickerBox : Control
	{
		private const string PART_button = "PART_button";

		private ExtendedButton? button;

		public event EventHandler<Color>? ColourChanged;

		static ColourPickerBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ColourPickerBox), new FrameworkPropertyMetadata(typeof(ColourPickerBox)));
		}

		#region Properties

		public Color Colour
		{
			get => (Color)GetValue(ColourProperty);
			set => SetValue(ColourProperty, value);
        }

        public static readonly DependencyProperty ColourProperty =
            DependencyProperty.Register(
                nameof(Colour),
                typeof(Color),
                typeof(ColourPickerBox),
                new FrameworkPropertyMetadata(null));

        public Color TemporaryColour
        {
            get => (Color)GetValue(TemporaryColourProperty);
            set => SetValue(TemporaryColourProperty, value);
        }

        public static readonly DependencyProperty TemporaryColourProperty =
            DependencyProperty.Register(
                nameof(TemporaryColour),
                typeof(Color),
                typeof(ColourPickerBox),
                new FrameworkPropertyMetadata(null));

        #endregion Properties

        public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (button != null)
			{
				button.Click -= Button_Click;
			}

			if (Template.FindName(PART_button, this) is ExtendedButton bt)
			{
				button = bt;
			}

			if (button != null)
			{
				button.Click += Button_Click;
			}
			else
			{
				throw new InvalidOperationException("Template missing rquired UI elements");
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
            Colour = new DialogueService().ShowColourPickerDialogue(Colour, colour => TemporaryColour = colour);
		}
    }
}