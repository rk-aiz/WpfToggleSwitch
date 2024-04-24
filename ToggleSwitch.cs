using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace WpfToggleSwitch
{
    /// <summary>
    /// Code behind only ToggleSwitch for PowerShell WPF execution
    /// </summary>
    public sealed class ToggleSwitch : ButtonBase
    {
        private const double _ratioOfEllipseRadiusToSwitchHeight = 0.375;
        private const double _ellipseShrinkScale = 0.95;
        private ScaleTransform _ellipseScaleTransform = new ScaleTransform(_ellipseShrinkScale, _ellipseShrinkScale);
        private TranslateTransform _ellipseTranslateTransform = new TranslateTransform();

        public ToggleSwitch()
        {
            TransformGroup tg = new TransformGroup();
            tg.Children.Add(_ellipseScaleTransform);
            tg.Children.Add(_ellipseTranslateTransform);

            EllipseTransformGroup = tg;

            Style = CreateStyle();
        }

        private void BeginEllipseTranslateAnimation(int duration = 300)
        {
            if (IsOn)
            {
                var anim = new DoubleAnimation
                {
                    From = (SwitchWidth * -0.5) + (SwitchHeight * 0.5),
                    To = 0,
                    Duration = TimeSpan.FromMilliseconds(duration),
                    EasingFunction = new CircleEase
                    {
                        EasingMode = EasingMode.EaseOut
                    }
                };
                _ellipseTranslateTransform.BeginAnimation(TranslateTransform.XProperty, anim, HandoffBehavior.SnapshotAndReplace);
            }
            else
            {
                var anim = new DoubleAnimation
                {
                    From = (SwitchWidth * 0.5) - (SwitchHeight * 0.5),
                    To = 0,
                    Duration = TimeSpan.FromMilliseconds(duration),
                    EasingFunction = new CircleEase
                    {
                        EasingMode = EasingMode.EaseOut
                    }
                };
                _ellipseTranslateTransform.BeginAnimation(TranslateTransform.XProperty, anim, HandoffBehavior.SnapshotAndReplace);
            }
        }

        private void BeginEllipseScaleAnimation(double percent = 1.0, int durationMillis = 100, IEasingFunction? easing = null, HandoffBehavior handoff = HandoffBehavior.SnapshotAndReplace)
        {
            var anim = new DoubleAnimation
            {
                To = percent,
                Duration = TimeSpan.FromMilliseconds(durationMillis),
                EasingFunction = easing
            };

            _ellipseScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, anim, handoff);
            _ellipseScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, anim, handoff);
        }

        protected override void OnClick()
        {
            IsOn = !IsOn;
            base.OnClick();
        }

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            BeginEllipseScaleAnimation(1.0);
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            BeginEllipseScaleAnimation(_ellipseShrinkScale, 100, new SineEase { EasingMode = EasingMode.EaseOut });
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            BeginEllipseScaleAnimation(_ellipseShrinkScale, 0);
            base.OnMouseDown(e);
        }

        protected override void OnLostMouseCapture(System.Windows.Input.MouseEventArgs e)
        {
            BeginEllipseScaleAnimation(1.0, 200, new ElasticEase { Springiness = 10, Oscillations = 0, EasingMode = EasingMode.EaseIn }, HandoffBehavior.Compose);
            base.OnLostMouseCapture(e);
        }

        private static Style CreateStyle()
        {
            var ellipseSizeBinding = new TemplateBindingExtension(SwitchHeightProperty)
            {
                Converter = MultiplicationValueConverter.I,
                ConverterParameter = _ratioOfEllipseRadiusToSwitchHeight * 2
            };
            var ellipseRadiusBinding = new TemplateBindingExtension(SwitchHeightProperty)
            {
                Converter = MultiplicationValueConverter.I,
                ConverterParameter = _ratioOfEllipseRadiusToSwitchHeight
            };

            var ellipse = new FrameworkElementFactory(typeof(Rectangle), "ellipse");
            ellipse.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
            ellipse.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Left);
            ellipse.SetValue(HeightProperty, ellipseSizeBinding);
            ellipse.SetValue(WidthProperty, ellipseSizeBinding);
            ellipse.SetValue(MarginProperty, new TemplateBindingExtension(SwitchHeightProperty){
                Converter = DoubleToThicknessConverter.I,
                ConverterParameter = 0.5 - _ratioOfEllipseRadiusToSwitchHeight 
            });
            ellipse.SetValue(Shape.FillProperty, new TemplateBindingExtension(ForegroundProperty));
            ellipse.SetValue(Rectangle.RadiusXProperty, ellipseRadiusBinding);
            ellipse.SetValue(Rectangle.RadiusYProperty, ellipseRadiusBinding);
            ellipse.SetValue(RenderTransformProperty, new TemplateBindingExtension(EllipseTransformGroupProperty));
            ellipse.SetValue(RenderTransformOriginProperty, new Point(0.5, 0.5));

            var radiusBinding = new TemplateBindingExtension(SwitchHeightProperty)
            {
                Converter = MultiplicationValueConverter.I,
                ConverterParameter = 0.5
            };
            var heightBinding = new TemplateBindingExtension(SwitchHeightProperty);
            var widthBinding = new TemplateBindingExtension(SwitchWidthProperty);

            var background = new FrameworkElementFactory(typeof(Rectangle), "background");
            background.SetValue(HeightProperty, heightBinding);
            background.SetValue(WidthProperty, widthBinding);
            background.SetValue(Shape.FillProperty, new TemplateBindingExtension(BackgroundProperty));
            background.SetValue(Rectangle.RadiusXProperty, radiusBinding);
            background.SetValue(Rectangle.RadiusYProperty, radiusBinding);

            var highlight = new FrameworkElementFactory(typeof(Rectangle), "highlight");
            highlight.SetValue(VisibilityProperty, Visibility.Collapsed);
            highlight.SetValue(HeightProperty, heightBinding);
            highlight.SetValue(WidthProperty, widthBinding);
            highlight.SetValue(Shape.FillProperty, new TemplateBindingExtension(HighlightBrushProperty));
            highlight.SetValue(Rectangle.RadiusXProperty, radiusBinding);
            highlight.SetValue(Rectangle.RadiusYProperty, radiusBinding);

            var border = new FrameworkElementFactory(typeof(Rectangle), "border");
            border.SetValue(HeightProperty, heightBinding);
            border.SetValue(WidthProperty, widthBinding);
            border.SetValue(Shape.FillProperty, Brushes.Transparent);
            border.SetValue(Shape.StrokeProperty, new TemplateBindingExtension(BorderBrushProperty));
            border.SetValue(Shape.StrokeThicknessProperty, new TemplateBindingExtension(BorderThicknessProperty));
            border.SetValue(Rectangle.RadiusXProperty, radiusBinding);
            border.SetValue(Rectangle.RadiusYProperty, radiusBinding);

            var switchArea = new FrameworkElementFactory(typeof(Grid), "switchGrid");
            switchArea.SetValue(MinHeightProperty, heightBinding); 
            switchArea.SetValue(MarginProperty, new TemplateBindingExtension(PaddingProperty));
            switchArea.AppendChild(background);
            switchArea.AppendChild(highlight);
            switchArea.AppendChild(border);
            switchArea.AppendChild(ellipse);

            var hitArea = new FrameworkElementFactory(typeof(Border), "hitArea");
            hitArea.SetValue(BackgroundProperty, Brushes.Transparent);

            var root = new FrameworkElementFactory(typeof(Grid), "TemplateRoot");
            root.AppendChild(hitArea);
            root.AppendChild(switchArea);

            var isOnTrigger = new Trigger
            {
                Property = IsOnProperty,
                Value = true,
            };
            isOnTrigger.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Right, "ellipse"));
            isOnTrigger.Setters.Add(new Setter(Shape.FillProperty, Brushes.Black, "ellipse"));
            isOnTrigger.Setters.Add(new Setter(VisibilityProperty, Visibility.Collapsed, "background"));
            isOnTrigger.Setters.Add(new Setter(VisibilityProperty, Visibility.Visible, "highlight"));
            isOnTrigger.Setters.Add(new Setter(Shape.StrokeThicknessProperty, 0.0, "border"));

            var mouseOverBrush = new SolidColorBrush(new Color { A = 30, R = 0, G = 0, B = 0 });
            mouseOverBrush.Freeze();

            var mouseOverTrigger = new Trigger
            {
                Property = IsMouseOverProperty,
                Value = true,
            };
            mouseOverTrigger.Setters.Add(new Setter(Shape.FillProperty, mouseOverBrush, "border"));

            var ellipsePressedWidthBinding = new Binding("SwitchHeight")
            {
                Mode = BindingMode.OneWay,
                RelativeSource = RelativeSource.TemplatedParent,
                Converter = MultiplicationValueConverter.I,
                ConverterParameter = _ratioOfEllipseRadiusToSwitchHeight * 2.4
            };

            var pressedTrigger = new Trigger
            {
                Property = IsPressedProperty,
                Value = true,
            };
            pressedTrigger.Setters.Add(new Setter(WidthProperty, ellipsePressedWidthBinding, "ellipse"));

            var disabledTrigger = new Trigger
            {
                Property = IsEnabledProperty,
                Value = false,
            };
            disabledTrigger.Setters.Add(new Setter(OpacityProperty, 0.35));

            var ct = new ControlTemplate(typeof(ButtonBase))
            {
                VisualTree = root,
            };
            ct.Triggers.Add(isOnTrigger);
            ct.Triggers.Add(mouseOverTrigger);
            ct.Triggers.Add(pressedTrigger);
            ct.Triggers.Add(disabledTrigger);

            var style = new Style(typeof(ButtonBase));
            style.Setters.Add(new Setter(TemplateProperty, ct));
            style.Setters.Add(new Setter(HorizontalAlignmentProperty, HorizontalAlignment.Center));
            style.Setters.Add(new Setter(VerticalAlignmentProperty, VerticalAlignment.Center));
            style.Setters.Add(new Setter(BorderThicknessProperty, new Thickness(1)));
            style.Setters.Add(new Setter(BackgroundProperty, Brushes.Transparent));
            style.Setters.Add(new Setter(ForegroundProperty, SystemColors.ControlDarkBrush));
            style.Setters.Add(new Setter(BorderBrushProperty, SystemColors.ActiveBorderBrush));
            return style;
        }

        /**
         * Dependency property changed callback(s)
         */
        private static void IsOnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var source = d as ToggleSwitch;
            if ((bool)e.OldValue == (bool)e.NewValue || source == null) return;

            source.BeginEllipseTranslateAnimation();
        }

        /**
         * Dependency properties
         */
        public bool IsOn
        {
            get { return (bool)GetValue(IsOnProperty); }
            set { SetValue(IsOnProperty, value); }
        }

        public static readonly DependencyProperty IsOnProperty =
            DependencyProperty.Register("IsOn",
                typeof(bool), typeof(ToggleSwitch), new PropertyMetadata(false, IsOnChanged));

        public Brush HighlightBrush
        {
            get { return (Brush)GetValue(HighlightBrushProperty); }
            set { SetValue(HighlightBrushProperty, value); }
        }

        public static readonly DependencyProperty HighlightBrushProperty =
            DependencyProperty.Register("HighlightBrush",
                typeof(Brush), typeof(ToggleSwitch), new PropertyMetadata(new SolidColorBrush(new Color { A = 255, R = 70, G = 160, B = 255 })));

        public double SwitchWidth
        {
            get { return (double)GetValue(SwitchWidthProperty); }
            set { SetValue(SwitchWidthProperty, value); }
        }

        public static readonly DependencyProperty SwitchWidthProperty =
            DependencyProperty.Register("SwitchWidth",
                typeof(double), typeof(ToggleSwitch), new PropertyMetadata(40.0));

        public double SwitchHeight
        {
            get { return (double)GetValue(SwitchHeightProperty); }
            set { SetValue(SwitchHeightProperty, value); }
        }

        public static readonly DependencyProperty SwitchHeightProperty =
            DependencyProperty.Register("SwitchHeight",
                typeof(double), typeof(ToggleSwitch), new PropertyMetadata(20.0));

        public TransformGroup EllipseTransformGroup
        {
            get { return (TransformGroup)GetValue(EllipseTransformGroupProperty); }
            set { SetValue(EllipseTransformGroupProperty, value); }
        }

        public static readonly DependencyProperty EllipseTransformGroupProperty =
            DependencyProperty.Register("EllipseTransformGroup", typeof(TransformGroup), typeof(ToggleSwitch), null);


        [ValueConversion(typeof(double), typeof(Thickness))]
        private class DoubleToThicknessConverter : IValueConverter
        {
            public static DoubleToThicknessConverter I = new DoubleToThicknessConverter();

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is double || parameter is double)
                    return new Thickness((double)value * (double)parameter);
                else
                    throw new ArgumentException();
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        [ValueConversion(typeof(double), typeof(double))]
        private class MultiplicationValueConverter : IValueConverter
        {
            public static MultiplicationValueConverter I = new MultiplicationValueConverter();

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is double || parameter is double)
                    return (double)value * (double)parameter;
                else
                    throw new ArgumentException();
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}
