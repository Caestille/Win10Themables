﻿using System;
using System.Reflection;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using Windows.Win32;
using Windows.Win32.Foundation;
using ControlzEx;
using ControlzEx.Native;
using MahApps.Metro.Automation.Peers;
using MahApps.Metro.ValueBoxes;
using MahApps.Metro.Controls;
using System.Collections;

namespace ModernThemables.Controls
{
	/// <summary>
	/// An extended Window class.
	/// </summary>
	[TemplatePart(Name = PART_Icon, Type = typeof(UIElement))]
	[TemplatePart(Name = PART_TitleBar, Type = typeof(UIElement))]
	[TemplatePart(Name = PART_WindowTitleBackground, Type = typeof(UIElement))]
	[TemplatePart(Name = PART_Content, Type = typeof(MetroContentControl))]
	[TemplatePart(Name = PART_WindowTitleThumb, Type = typeof(Thumb))]
	[TemplatePart(Name = PART_LeftWindowCommands, Type = typeof(ContentPresenter))]
	[TemplatePart(Name = PART_RightWindowCommands, Type = typeof(ContentPresenter))]
	[TemplatePart(Name = PART_WindowButtonCommands, Type = typeof(ContentPresenter))]
	[TemplatePart(Name = PART_SettingsCloseButton, Type = typeof(Button))]
	[TemplatePart(Name = PART_SettingsCloseRegion, Type = typeof(Button))]
	[TemplatePart(Name = PART_ThemingMenu, Type = typeof(ThemingControl))]
	public class ThemableWindow2 : WindowChromeWindow
	{
		private const string PART_Icon = "PART_Icon";
		private const string PART_WindowTitleThumb = "PART_WindowTitleThumb";
		private const string PART_TitleBar = "PART_TitleBar";
		private const string PART_WindowTitleBackground = "PART_WindowTitleBackground";
		private const string PART_Content = "PART_Content";
		private const string PART_LeftWindowCommands = "PART_LeftWindowCommands";
		private const string PART_RightWindowCommands = "PART_RightWindowCommands";
		private const string PART_WindowButtonCommands = "PART_WindowButtonCommands";
		private const string PART_SettingsCloseButton = "PART_SettingsCloseButton";
		private const string PART_SettingsCloseRegion = "PART_SettingsCloseRegion";
		private const string PART_ThemingMenu = "PART_ThemingMenu";

		private FrameworkElement? icon;
		private Thumb? windowTitleThumb;
		private UIElement? titleBar;
		private UIElement? titleBarBackground;
		private ContentPresenter? LeftWindowCommandsPresenter;
		private ContentPresenter? RightWindowCommandsPresenter;
		private ContentPresenter? WindowButtonCommandsPresenter;
		private Button? SettingsCloseButton;
		private Button? SettingsCloseRegion;
		private ThemingControl? ThemingMenu;

		#region Properties

		/// <summary>
		/// Get or sets whether the TitleBar icon is visible or not.
		/// </summary>
		public bool ShowThemingMenu
		{
			get => (bool)this.GetValue(ShowThemingMenuProperty);
			set => this.SetValue(ShowThemingMenuProperty, BooleanBoxes.Box(value));
		}
		public static readonly DependencyProperty ShowThemingMenuProperty = DependencyProperty.Register(
			nameof(ShowThemingMenu),
			typeof(bool),
			typeof(ThemableWindow2),
			new PropertyMetadata(BooleanBoxes.FalseBox));

		/// <summary>
		/// Get or sets whether the TitleBar icon is visible or not.
		/// </summary>
		public bool ShowIcon
		{
			get => (bool)this.GetValue(ShowIconProperty);
			set => this.SetValue(ShowIconProperty, BooleanBoxes.Box(value));
		}
		public static readonly DependencyProperty ShowIconProperty = DependencyProperty.Register(
			nameof(ShowIcon),
			typeof(bool),
			typeof(ThemableWindow2),
			new PropertyMetadata(BooleanBoxes.TrueBox, OnShowIconPropertyChangedCallback));

		private static void OnShowIconPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var window = (ThemableWindow2)d;
			if (e.NewValue != e.OldValue)
			{
				window.UpdateIconVisibility();
			}
		}

		/// <summary>
		/// Gets or sets the edge mode for the TitleBar icon.
		/// </summary>
		public EdgeMode IconEdgeMode
		{
			get => (EdgeMode)this.GetValue(IconEdgeModeProperty);
			set => this.SetValue(IconEdgeModeProperty, value);
		}
		public static readonly DependencyProperty IconEdgeModeProperty = DependencyProperty.Register(
			nameof(IconEdgeMode),
			typeof(EdgeMode),
			typeof(ThemableWindow2),
			new PropertyMetadata(EdgeMode.Aliased));

		/// <summary>
		/// Gets or sets the bitmap scaling mode for the TitleBar icon.
		/// </summary>
		public BitmapScalingMode IconBitmapScalingMode
		{
			get => (BitmapScalingMode)this.GetValue(IconBitmapScalingModeProperty);
			set => this.SetValue(IconBitmapScalingModeProperty, value);
		}
		public static readonly DependencyProperty IconBitmapScalingModeProperty = DependencyProperty.Register(
			nameof(IconBitmapScalingMode),
			typeof(BitmapScalingMode),
			typeof(ThemableWindow2),
			new PropertyMetadata(BitmapScalingMode.HighQuality));

		/// <summary>
		/// Gets or sets the scaling mode for the TitleBar icon.
		/// </summary>
		public MultiFrameImageMode IconScalingMode
		{
			get => (MultiFrameImageMode)this.GetValue(IconScalingModeProperty);
			set => this.SetValue(IconScalingModeProperty, value);
		}
		public static readonly DependencyProperty IconScalingModeProperty = DependencyProperty.Register(
			nameof(IconScalingMode),
			typeof(MultiFrameImageMode),
			typeof(ThemableWindow2),
			new FrameworkPropertyMetadata(MultiFrameImageMode.ScaleDownLargerFrame, FrameworkPropertyMetadataOptions.AffectsRender));

		/// <summary>
		/// Gets or sets whether the TitleBar is visible or not.
		/// </summary>
		public bool ShowTitleBar
		{
			get => (bool)this.GetValue(ShowTitleBarProperty);
			set => this.SetValue(ShowTitleBarProperty, BooleanBoxes.Box(value));
		}
		public static readonly DependencyProperty ShowTitleBarProperty = DependencyProperty.Register(
			nameof(ShowTitleBar),
			typeof(bool),
			typeof(ThemableWindow2),
			new PropertyMetadata(BooleanBoxes.TrueBox, OnShowTitleBarPropertyChangedCallback));

		/// <summary>
		/// Gets or sets a value that indicates whether the system menu should popup with left mouse click on the window icon.
		/// </summary>
		public bool ShowSystemMenu
		{
			get => (bool)this.GetValue(ShowSystemMenuProperty);
			set => this.SetValue(ShowSystemMenuProperty, BooleanBoxes.Box(value));
		}
		public static readonly DependencyProperty ShowSystemMenuProperty = DependencyProperty.Register(
			nameof(ShowSystemMenu),
			typeof(bool),
			typeof(ThemableWindow2),
			new PropertyMetadata(BooleanBoxes.TrueBox));

		/// <summary>
		/// Gets or sets a value that indicates whether the system menu should popup with right mouse click if the mouse position is on title bar or on the entire window if it has no TitleBar (and no TitleBar height).
		/// </summary>
		public bool ShowSystemMenuOnRightClick
		{
			get => (bool)this.GetValue(ShowSystemMenuOnRightClickProperty);
			set => this.SetValue(ShowSystemMenuOnRightClickProperty, BooleanBoxes.Box(value));
		}
		public static readonly DependencyProperty ShowSystemMenuOnRightClickProperty = DependencyProperty.Register(
			nameof(ShowSystemMenuOnRightClick),
			typeof(bool),
			typeof(ThemableWindow2),
			new PropertyMetadata(BooleanBoxes.TrueBox));

		/// <summary>
		/// Gets or sets the TitleBar's height.
		/// </summary>
		public int TitleBarHeight
		{
			get => (int)this.GetValue(TitleBarHeightProperty);
			set => this.SetValue(TitleBarHeightProperty, value);
		}
		public static readonly DependencyProperty TitleBarHeightProperty = DependencyProperty.Register(
			nameof(TitleBarHeight),
			typeof(int),
			typeof(ThemableWindow2),
			new PropertyMetadata(30, TitleBarHeightPropertyChangedCallback));

		private static void TitleBarHeightPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != e.OldValue)
			{
				((ThemableWindow2)d).UpdateTitleBarElementsVisibility();
			}
		}

		/// <summary>
		/// Gets or sets the horizontal alignment of the title.
		/// </summary>
		public HorizontalAlignment TitleAlignment
		{
			get => (HorizontalAlignment)this.GetValue(TitleAlignmentProperty);
			set => this.SetValue(TitleAlignmentProperty, value);
		}
		public static readonly DependencyProperty TitleAlignmentProperty = DependencyProperty.Register(
			nameof(TitleAlignment),
			typeof(HorizontalAlignment),
			typeof(ThemableWindow2),
			new PropertyMetadata(HorizontalAlignment.Stretch, OnTitleAlignmentChanged));

		private static void OnTitleAlignmentChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			if (e.OldValue != e.NewValue)
			{
				var window = (ThemableWindow2)dependencyObject;

				window.SizeChanged -= window.ThemableWindow2_SizeChanged;
				if (e.NewValue is HorizontalAlignment horizontalAlignment && horizontalAlignment == HorizontalAlignment.Center && window.titleBar != null)
				{
					window.SizeChanged += window.ThemableWindow2_SizeChanged;
				}
			}
		}

		/// <summary>
		/// Gets or sets the brush used for the TitleBar's foreground.
		/// </summary>
		public Brush? TitleForeground
		{
			get => (Brush?)this.GetValue(TitleForegroundProperty);
			set => this.SetValue(TitleForegroundProperty, value);
		}
		public static readonly DependencyProperty TitleForegroundProperty = DependencyProperty.Register(
			nameof(TitleForeground),
			typeof(Brush),
			typeof(ThemableWindow2));

		/// <summary>
		/// Gets or sets the brush used for the TitleBar's foreground.
		/// </summary>
		public Brush? NonActiveTitleForeground
		{
			get => (Brush?)this.GetValue(NonActiveTitleForegroundProperty);
			set => this.SetValue(NonActiveTitleForegroundProperty, value);
		}
		public static readonly DependencyProperty NonActiveTitleForegroundProperty = DependencyProperty.Register(
			nameof(NonActiveTitleForeground),
			typeof(Brush),
			typeof(ThemableWindow2));

		/// <summary>
		/// Gets or sets the <see cref="DataTemplate"/> for the <see cref="Window.Title"/>.
		/// </summary>
		public DataTemplate? TitleTemplate
		{
			get => (DataTemplate?)this.GetValue(TitleTemplateProperty);
			set => this.SetValue(TitleTemplateProperty, value);
		}
		public static readonly DependencyProperty TitleTemplateProperty = DependencyProperty.Register(
			nameof(TitleTemplate),
			typeof(DataTemplate),
			typeof(ThemableWindow2),
			new PropertyMetadata(null));

		/// <summary>
		/// Gets or sets the brush used for the background of the TitleBar.
		/// </summary>
		public Brush WindowTitleBrush
		{
			get => (Brush)this.GetValue(WindowTitleBrushProperty);
			set => this.SetValue(WindowTitleBrushProperty, value);
		}
		public static readonly DependencyProperty WindowTitleBrushProperty = DependencyProperty.Register(
			nameof(WindowTitleBrush),
			typeof(Brush),
			typeof(ThemableWindow2),
			new PropertyMetadata(Brushes.Transparent));

		/// <summary>
		/// Gets or sets the non-active brush used for the background of the TitleBar.
		/// </summary>
		public Brush NonActiveWindowTitleBrush
		{
			get => (Brush)this.GetValue(NonActiveWindowTitleBrushProperty);
			set => this.SetValue(NonActiveWindowTitleBrushProperty, value);
		}
		public static readonly DependencyProperty NonActiveWindowTitleBrushProperty = DependencyProperty.Register(
			nameof(NonActiveWindowTitleBrush),
			typeof(Brush),
			typeof(ThemableWindow2),
			new PropertyMetadata(Brushes.Gray));

		/// <summary>
		/// Gets or sets the non-active brush used for the border of the window.
		/// </summary>
		public Brush NonActiveBorderBrush
		{
			get => (Brush)this.GetValue(NonActiveBorderBrushProperty);
			set => this.SetValue(NonActiveBorderBrushProperty, value);
		}
		public static readonly DependencyProperty NonActiveBorderBrushProperty = DependencyProperty.Register(
			nameof(NonActiveBorderBrush),
			typeof(Brush),
			typeof(ThemableWindow2),
			new PropertyMetadata(Brushes.Gray));

		/// <summary>
		/// Gets or sets the <see cref="DataTemplate"/> for the icon on the TitleBar.
		/// </summary>
		public DataTemplate? IconTemplate
		{
			get => (DataTemplate?)this.GetValue(IconTemplateProperty);
			set => this.SetValue(IconTemplateProperty, value);
		}
		public static readonly DependencyProperty IconTemplateProperty = DependencyProperty.Register(
			nameof(IconTemplate),
			typeof(DataTemplate),
			typeof(ThemableWindow2),
			new PropertyMetadata(null, (o, e) =>
			{
				if (e.NewValue != e.OldValue)
				{
					(o as ThemableWindow2)?.UpdateIconVisibility();
				}
			}));

		/// <summary>
		/// Gets or sets the <see cref="WindowCommands"/> host on the left side of the TitleBar.
		/// </summary>
		public WindowCommands? LeftWindowCommands
		{
			get => (WindowCommands?)this.GetValue(LeftWindowCommandsProperty);
			set => this.SetValue(LeftWindowCommandsProperty, value);
		}
		public static readonly DependencyProperty LeftWindowCommandsProperty = DependencyProperty.Register(
			nameof(LeftWindowCommands),
			typeof(WindowCommands),
			typeof(ThemableWindow2),
			new PropertyMetadata(null, OnLeftWindowCommandsPropertyChanged));

		private static void OnLeftWindowCommandsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue is WindowCommands windowCommands)
			{
				AutomationProperties.SetName(windowCommands, nameof(LeftWindowCommands));
			}

			UpdateLogicalChildren(d, e);
		}

		/// <summary>
		/// Gets or sets the <see cref="WindowCommands"/> host on the right side of the TitleBar.
		/// </summary>
		public WindowCommands? RightWindowCommands
		{
			get => (WindowCommands?)this.GetValue(RightWindowCommandsProperty);
			set => this.SetValue(RightWindowCommandsProperty, value);
		}

		public static readonly DependencyProperty RightWindowCommandsProperty = DependencyProperty.Register(
			nameof(RightWindowCommands),
			typeof(WindowCommands),
			typeof(ThemableWindow2),
			new PropertyMetadata(null, OnRightWindowCommandsPropertyChanged));

		private static void OnRightWindowCommandsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue is WindowCommands windowCommands)
			{
				AutomationProperties.SetName(windowCommands, nameof(RightWindowCommands));
			}

			UpdateLogicalChildren(d, e);
		}

		/// <summary>
		/// Gets or sets the <see cref="WindowButtonCommands"/> host that shows the minimize/maximize/restore/close buttons.
		/// </summary>
		public WindowButtonCommands? WindowButtonCommands
		{
			get => (WindowButtonCommands?)this.GetValue(WindowButtonCommandsProperty);
			set => this.SetValue(WindowButtonCommandsProperty, value);
		}
		public static readonly DependencyProperty WindowButtonCommandsProperty = DependencyProperty.Register(
			nameof(WindowButtonCommands),
			typeof(WindowButtonCommands),
			typeof(ThemableWindow2),
			new PropertyMetadata(null, UpdateLogicalChildren));

		private static void OnShowTitleBarPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != e.OldValue)
			{
				((ThemableWindow2)d).UpdateTitleBarElementsVisibility();
			}
		}

		/// <summary>
		/// Gets or sets whether the whole window is draggable.
		/// </summary>
		public bool IsWindowDraggable
		{
			get => (bool)this.GetValue(IsWindowDraggableProperty);
			set => this.SetValue(IsWindowDraggableProperty, BooleanBoxes.Box(value));
		}
		public static readonly DependencyProperty IsWindowDraggableProperty = DependencyProperty.Register(
			nameof(IsWindowDraggable),
			typeof(bool),
			typeof(ThemableWindow2),
			new PropertyMetadata(BooleanBoxes.TrueBox));

		/// <inheritdoc />
		protected override IEnumerator LogicalChildren
		{
			get
			{
				// cheat, make a list with all logical content and return the enumerator
				ArrayList children = new ArrayList();
				if (this.Content != null)
				{
					children.Add(this.Content);
				}

				if (this.LeftWindowCommands != null)
				{
					children.Add(this.LeftWindowCommands);
				}

				if (this.RightWindowCommands != null)
				{
					children.Add(this.RightWindowCommands);
				}

				if (this.WindowButtonCommands != null)
				{
					children.Add(this.WindowButtonCommands);
				}

				return children.GetEnumerator();
			}
		}

		#endregion

		static ThemableWindow2()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ThemableWindow2), new FrameworkPropertyMetadata(typeof(ThemableWindow2)));

			IconProperty.OverrideMetadata(
				typeof(ThemableWindow2),
				new FrameworkPropertyMetadata(
					(o, e) =>
					{
						if (e.NewValue != e.OldValue)
						{
							(o as ThemableWindow2)?.UpdateIconVisibility();
						}
					}));
		}

		/// <summary>
		/// Initializes a new instance of the MahApps.Metro.Controls.ThemableWindow2 class.
		/// </summary>
		public ThemableWindow2()
		{
			this.DataContextChanged += this.ThemableWindow2_DataContextChanged;
		}

		private void UpdateIconVisibility()
		{
			var isVisible = (this.Icon is not null || this.IconTemplate is not null)
							&& (!this.ShowTitleBar || (this.ShowTitleBar));
			this.icon?.SetCurrentValue(VisibilityProperty, isVisible ? Visibility.Visible : Visibility.Collapsed);
		}

		private void UpdateTitleBarElementsVisibility()
		{
			this.UpdateIconVisibility();

			var newVisibility = this.TitleBarHeight > 0 && this.ShowTitleBar ? Visibility.Visible : Visibility.Collapsed;

			this.titleBar?.SetCurrentValue(VisibilityProperty, newVisibility);
			this.titleBarBackground?.SetCurrentValue(VisibilityProperty, newVisibility);

			this.SetWindowEvents();
		}

		private void ThemableWindow2_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			// MahApps add these controls to the window with AddLogicalChild method.
			// This has the side effect that the DataContext doesn't update, so do this now here.
			if (this.LeftWindowCommands != null)
			{
				this.LeftWindowCommands.DataContext = this.DataContext;
			}

			if (this.RightWindowCommands != null)
			{
				this.RightWindowCommands.DataContext = this.DataContext;
			}

			if (this.WindowButtonCommands != null)
			{
				this.WindowButtonCommands.DataContext = this.DataContext;
			}
		}

		private void ThemableWindow2_SizeChanged(object sender, RoutedEventArgs e)
		{
			// this all works only for centered title
			if (this.TitleAlignment != HorizontalAlignment.Center
				|| this.titleBar is null)
			{
				return;
			}

			// Half of this ThemableWindow2
			var halfDistance = this.ActualWidth / 2;
			// Distance between center and left/right
			var margin = (Thickness)this.titleBar.GetValue(MarginProperty);
			var distanceToCenter = (this.titleBar.DesiredSize.Width - margin.Left - margin.Right) / 2;

			var iconWidth = this.icon?.ActualWidth ?? 0;
			var leftWindowCommandsWidth = this.LeftWindowCommands?.ActualWidth ?? 0;
			var rightWindowCommandsWidth = this.RightWindowCommands?.ActualWidth ?? 0;
			var windowButtonCommandsWith = this.WindowButtonCommands?.ActualWidth ?? 0;

			// Distance between right edge from LeftWindowCommands to left window side
			var distanceFromLeft = iconWidth + leftWindowCommandsWidth;
			// Distance between left edge from RightWindowCommands to right window side
			var distanceFromRight = rightWindowCommandsWidth + windowButtonCommandsWith;
			// Margin
			const double horizontalMargin = 5.0;

			var dLeft = distanceFromLeft + distanceToCenter + horizontalMargin;
			var dRight = distanceFromRight + distanceToCenter + horizontalMargin;
			if ((dLeft < halfDistance) && (dRight < halfDistance))
			{
				this.titleBar.SetCurrentValue(MarginProperty, default(Thickness));
				Grid.SetColumn(this.titleBar, 0);
				Grid.SetColumnSpan(this.titleBar, 3);
			}
			else
			{
				this.titleBar.SetCurrentValue(MarginProperty, new Thickness(leftWindowCommandsWidth, 0, rightWindowCommandsWidth, 0));
				Grid.SetColumn(this.titleBar, 1);
				Grid.SetColumnSpan(this.titleBar, 1);
			}
		}

		private static void UpdateLogicalChildren(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			if (dependencyObject is not ThemableWindow2 window)
			{
				return;
			}

			if (e.OldValue is FrameworkElement oldChild)
			{
				window.RemoveLogicalChild(oldChild);
			}

			if (e.NewValue is FrameworkElement newChild)
			{
				window.AddLogicalChild(newChild);
				newChild.DataContext = window.DataContext;
			}
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			this.LeftWindowCommandsPresenter = this.GetTemplateChild(PART_LeftWindowCommands) as ContentPresenter;
			this.RightWindowCommandsPresenter = this.GetTemplateChild(PART_RightWindowCommands) as ContentPresenter;
			this.WindowButtonCommandsPresenter = this.GetTemplateChild(PART_WindowButtonCommands) as ContentPresenter;
			this.SettingsCloseButton = this.GetTemplateChild(PART_SettingsCloseButton) as Button;
			this.SettingsCloseRegion = this.GetTemplateChild(PART_SettingsCloseRegion) as Button;
			this.ThemingMenu = this.GetTemplateChild(PART_ThemingMenu) as ThemingControl;

			this.LeftWindowCommands ??= new WindowCommands();
			this.RightWindowCommands ??= new WindowCommands();
			this.WindowButtonCommands ??= new WindowButtonCommands();

			//this.LeftWindowCommands.SetValue(WindowCommands.ParentWindowPropertyKey, this);
			//this.RightWindowCommands.SetValue(WindowCommands.ParentWindowPropertyKey, this);
			this.WindowButtonCommands.SetValue(WindowButtonCommands.ParentWindowPropertyKey, this);

			this.icon = this.GetTemplateChild(PART_Icon) as FrameworkElement;
			this.titleBar = this.GetTemplateChild(PART_TitleBar) as UIElement;
			this.titleBarBackground = this.GetTemplateChild(PART_WindowTitleBackground) as UIElement;
			this.windowTitleThumb = this.GetTemplateChild(PART_WindowTitleThumb) as Thumb;

			this.UpdateTitleBarElementsVisibility();
		}

		/// <summary>
		/// Creates AutomationPeer (<see cref="UIElement.OnCreateAutomationPeer"/>)
		/// </summary>
		protected override AutomationPeer OnCreateAutomationPeer()
		{
			return new MetroWindowAutomationPeer(this);
		}

		protected internal IntPtr CriticalHandle
		{
			get
			{
				this.VerifyAccess();
				var value = typeof(Window)
					.GetProperty("CriticalHandle", BindingFlags.NonPublic | BindingFlags.Instance)?
					.GetValue(this, new object[0]) 
						?? IntPtr.Zero;
				return (IntPtr)value;
			}
		}

		private void ClearWindowEvents()
		{
			if (this.windowTitleThumb != null)
			{
				this.windowTitleThumb.PreviewMouseLeftButtonUp -= this.WindowTitleThumbOnPreviewMouseLeftButtonUp;
				this.windowTitleThumb.DragDelta -= this.WindowTitleThumbMoveOnDragDelta;
				this.windowTitleThumb.MouseDoubleClick -= this.WindowTitleThumbChangeWindowStateOnMouseDoubleClick;
				this.windowTitleThumb.MouseRightButtonUp -= this.WindowTitleThumbSystemMenuOnMouseRightButtonUp;
			}

			if (this.titleBar is IMetroThumb thumbContentControl)
			{
				thumbContentControl.PreviewMouseLeftButtonUp -= this.WindowTitleThumbOnPreviewMouseLeftButtonUp;
				thumbContentControl.DragDelta -= this.WindowTitleThumbMoveOnDragDelta;
				thumbContentControl.MouseDoubleClick -= this.WindowTitleThumbChangeWindowStateOnMouseDoubleClick;
				thumbContentControl.MouseRightButtonUp -= this.WindowTitleThumbSystemMenuOnMouseRightButtonUp;
			}

			if (this.icon != null)
			{
				this.icon.PreviewMouseLeftButtonUp -= this.WindowTitleThumbSystemMenuOnMouseRightButtonUp;
			}

			if (this.WindowButtonCommands != null)
			{
				this.WindowButtonCommands.ToggleThemeingMenu -= ToggleShowThemingMenu;
				this.WindowButtonCommands.MaximisingWindow -= OnMaximise;
				this.WindowButtonCommands.MaximisedWindow -= OnMaximised;
			}

			if (SettingsCloseButton != null)
			{
				SettingsCloseButton.Click -= CloseThemingMenu;
			}

			if (SettingsCloseRegion != null)
			{
				SettingsCloseRegion.Click -= CloseThemingMenu;
			}

			if (ThemingMenu != null)
			{
				ThemingMenu.InternalRequestClose -= ThemingMenu_InternalRequestClose;
			}

			this.SizeChanged -= this.ThemableWindow2_SizeChanged;
		}

		private void SetWindowEvents()
		{
			// clear all event handlers first
			this.ClearWindowEvents();

			if (this.titleBar is IMetroThumb thumbContentControl)
			{
				thumbContentControl.PreviewMouseLeftButtonUp += this.WindowTitleThumbOnPreviewMouseLeftButtonUp;
				thumbContentControl.DragDelta += this.WindowTitleThumbMoveOnDragDelta;
				thumbContentControl.MouseDoubleClick += this.WindowTitleThumbChangeWindowStateOnMouseDoubleClick;
				thumbContentControl.MouseRightButtonUp += this.WindowTitleThumbSystemMenuOnMouseRightButtonUp;
			}

			if (this.windowTitleThumb != null)
			{
				this.windowTitleThumb.PreviewMouseLeftButtonUp += this.WindowTitleThumbOnPreviewMouseLeftButtonUp;
				this.windowTitleThumb.DragDelta += this.WindowTitleThumbMoveOnDragDelta;
				this.windowTitleThumb.MouseDoubleClick += this.WindowTitleThumbChangeWindowStateOnMouseDoubleClick;
				this.windowTitleThumb.MouseRightButtonUp += this.WindowTitleThumbSystemMenuOnMouseRightButtonUp;
			}

			if (this.icon != null)
			{
				this.icon.PreviewMouseLeftButtonUp += this.WindowTitleThumbSystemMenuOnMouseRightButtonUp;
			}

			if (this.WindowButtonCommands != null)
			{
				this.WindowButtonCommands.ToggleThemeingMenu += ToggleShowThemingMenu;
				this.WindowButtonCommands.MaximisingWindow += OnMaximise;
				this.WindowButtonCommands.MaximisedWindow += OnMaximised;
			}

			if (SettingsCloseButton != null)
			{
				SettingsCloseButton.Click += CloseThemingMenu;
			}

			if (SettingsCloseRegion != null)
			{
				SettingsCloseRegion.Click += CloseThemingMenu;
			}

			if (ThemingMenu != null)
			{
				ThemingMenu.InternalRequestClose += ThemingMenu_InternalRequestClose;
			}

			// handle size if we have a Grid for the title (e.g. clean window have a centered title)
			if (this.titleBar != null && this.TitleAlignment == HorizontalAlignment.Center)
			{
				this.SizeChanged += this.ThemableWindow2_SizeChanged;
			}
		}

		private void WindowTitleThumbOnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			DoWindowTitleThumbOnPreviewMouseLeftButtonUp(this, e);
		}

		private void WindowTitleThumbMoveOnDragDelta(object sender, DragDeltaEventArgs dragDeltaEventArgs)
		{
			DoWindowTitleThumbMoveOnDragDelta(sender as IMetroThumb, this, dragDeltaEventArgs);
		}

		private void WindowTitleThumbChangeWindowStateOnMouseDoubleClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
		{
			DoWindowTitleThumbChangeWindowStateOnMouseDoubleClick(this, mouseButtonEventArgs);
		}

		private void WindowTitleThumbSystemMenuOnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			DoWindowTitleThumbSystemMenuOnMouseRightButtonUp(this, e);
		}

		private void ToggleShowThemingMenu(object sender, WindowEventHandlerArgs e)
		{
			ShowThemingMenu = !ShowThemingMenu;
			if (ShowThemingMenu && ThemingMenu != null)
			{
				ThemingMenu.FocusOnOpen();
			}
		}

		private void OnMaximise(object sender, WindowEventHandlerArgs e)
		{
			this.GlowColor = null;
		}

		private void OnMaximised(object sender, WindowEventHandlerArgs e)
		{
			SetResourceReference(GlowColorProperty, "ThemeMouseOverColour");
		}

		private void CloseThemingMenu(object sender, RoutedEventArgs e)
		{
			ShowThemingMenu = false;
			if (WindowButtonCommands != null)
			{
				WindowButtonCommands.IsThemingMenuVisible = false;
			}
		}

		private void ThemingMenu_InternalRequestClose(object? sender, EventArgs e)
		{
			CloseThemingMenu(sender, null);
		}

		internal static void DoWindowTitleThumbOnPreviewMouseLeftButtonUp(ThemableWindow2 window, MouseButtonEventArgs mouseButtonEventArgs)
		{
			if (mouseButtonEventArgs.Source == mouseButtonEventArgs.OriginalSource)
			{
				Mouse.Capture(null);
			}
		}

		internal static void DoWindowTitleThumbMoveOnDragDelta(IMetroThumb? thumb, ThemableWindow2? window, DragDeltaEventArgs dragDeltaEventArgs)
		{
			if (thumb is null)
			{
				throw new ArgumentNullException(nameof(thumb));
			}

			if (window is null)
			{
				throw new ArgumentNullException(nameof(window));
			}

			// drag only if IsWindowDraggable is set to true
			if (!window.IsWindowDraggable ||
				(!(Math.Abs(dragDeltaEventArgs.HorizontalChange) > 2) && !(Math.Abs(dragDeltaEventArgs.VerticalChange) > 2)))
			{
				return;
			}

			// This was taken from DragMove internal code
			window.VerifyAccess();

			// if the window is maximized dragging is only allowed on title bar (also if not visible)
			var windowIsMaximized = window.WindowState == WindowState.Maximized;
			var isMouseOnTitlebar = Mouse.GetPosition(thumb).Y <= window.TitleBarHeight && window.TitleBarHeight > 0;
			if (!isMouseOnTitlebar && windowIsMaximized)
			{
				return;
			}

			// for the touch usage
			PInvoke.ReleaseCapture();

			if (windowIsMaximized)
			{
				EventHandler? onWindowStateChanged = null;
				onWindowStateChanged = (sender, args) =>
				{
					window.StateChanged -= onWindowStateChanged;

					if (window.WindowState == WindowState.Normal)
					{
						Mouse.Capture(thumb, CaptureMode.Element);
					}
				};

				window.StateChanged -= onWindowStateChanged;
				window.StateChanged += onWindowStateChanged;
			}

			var wpfPoint = window.PointToScreen(Mouse.GetPosition(window));
			var x = (int)wpfPoint.X;
			var y = (int)wpfPoint.Y;
			PInvoke.SendMessage(new HWND(window.CriticalHandle), PInvoke.WM_NCLBUTTONDOWN, new WPARAM((nuint)HT.CAPTION), new IntPtr(x | (y << 16)));
		}

		internal static void DoWindowTitleThumbChangeWindowStateOnMouseDoubleClick(ThemableWindow2 window, MouseButtonEventArgs mouseButtonEventArgs)
		{
			// restore/maximize only with left button
			if (mouseButtonEventArgs.ChangedButton == MouseButton.Left)
			{
				// we can maximize or restore the window if the title bar height is set (also if title bar is hidden)
				var canResize = window.ResizeMode == ResizeMode.CanResizeWithGrip || window.ResizeMode == ResizeMode.CanResize;
				var mousePos = Mouse.GetPosition(window);
				var isMouseOnTitlebar = mousePos.Y <= window.TitleBarHeight && window.TitleBarHeight > 0;
				if (canResize && isMouseOnTitlebar)
				{
#pragma warning disable 618
					if (window.WindowState == WindowState.Normal)
					{
						ControlzEx.SystemCommands.MaximizeWindow(window);
					}
					else
					{
						ControlzEx.SystemCommands.RestoreWindow(window);
					}
#pragma warning restore 618
					mouseButtonEventArgs.Handled = true;
				}
			}
		}

		internal static void DoWindowTitleThumbSystemMenuOnMouseRightButtonUp(ThemableWindow2 window, MouseButtonEventArgs e)
		{
			if (window.ShowSystemMenuOnRightClick)
			{
				// show menu only if mouse pos is on title bar or if we have a window with none style and no title bar
				var mousePos = e.GetPosition(window);
				if ((mousePos.Y <= window.TitleBarHeight && window.TitleBarHeight > 0) || (window.WindowStyle == WindowStyle.None && window.TitleBarHeight <= 0))
				{
#pragma warning disable 618
					ControlzEx.SystemCommands.ShowSystemMenuPhysicalCoordinates(window, window.PointToScreen(mousePos));
#pragma warning restore 618
				}
			}
		}
	}
}