using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace SuperFramework.SuperWindows.WPFEffect
{
    /// <summary>
    /// 使用组合刷和光效果绘制具有揭示效果的控制边框。
    /// </summary>
    public class RevealBorderBrushExtension : MarkupExtension
    {
        /// <summary>
        /// The color to use for rendering in case the <see cref="MarkupExtension"/> can't work correctly.
        /// </summary>
        public Color FallbackColor { get; set; } = Colors.White;

        /// <summary>
        /// 获取或设置指定画笔基本背景颜色的值。
        /// </summary>
        public Color Color { get; set; } = Colors.White;

        public Transform Transform { get; set; } = Transform.Identity;

        public Transform RelativeTransform { get; set; } = Transform.Identity;

        public double Opacity { get; set; } = 1.0;

        public double Radius { get; set; } = 100.0;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // 如果没有服务，则直接返回。
            if (!(serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget service)) return null;
            // MarkupExtension 在样式模板中，返回 this 以延迟提供值。
            if (service.TargetObject.ToString().EndsWith("SharedDp")) return this;
            if (!(service.TargetObject is FrameworkElement element)) return this;
            if (DesignerProperties.GetIsInDesignMode(element)) return new SolidColorBrush(FallbackColor);

            var window = Window.GetWindow(element);
            if (window == null) return this;
            var brush = CreateBrush(window, element);
            return brush;
        }

        private Brush CreateBrush(Window window, FrameworkElement element)
        {
            var brush = new RadialGradientBrush(Colors.White, Colors.Transparent)
            {
                MappingMode = BrushMappingMode.Absolute,
                RadiusX = Radius,
                RadiusY = Radius,
                Opacity = Opacity,
                Transform = Transform,
                RelativeTransform = RelativeTransform,
            };
            window.MouseMove += OnMouseMove;
            window.Closed += OnClosed;
            return brush;

            void OnMouseMove(object sender, MouseEventArgs e)
            {
                var position = e.GetPosition(element);
                brush.GradientOrigin = position;
                brush.Center = position;
            }

            void OnClosed(object o, EventArgs eventArgs)
            {
                window.MouseMove -= OnMouseMove;
                window.Closed -= OnClosed;
            }
        }
    }
}
