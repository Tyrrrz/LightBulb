using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Tyrrrz.Extensions;

namespace LightBulb.Views.Controls
{
    public class RotatedPoint : Shape
    {
        private static object CoerceAngle(DependencyObject d, object baseValue)
        {
            if (baseValue is double angle)
            {
                return angle % 360.0;
            }

            return baseValue;
        }

        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register(nameof(Angle), typeof(double), typeof(RotatedPoint),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, null, CoerceAngle));

        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register(nameof(Size), typeof(double), typeof(RotatedPoint),
                new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.AffectsRender, null, CoerceAngle));

        public double Angle
        {
            get => (double) GetValue(AngleProperty);
            set => SetValue(AngleProperty, value);
        }

        public double Size
        {
            get => (double) GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }

        protected override Geometry DefiningGeometry => GetDefiningGeometry();

        private Geometry GetDefiningGeometry()
        {
            var offsetX = StrokeThickness + Size;
            var offsetY = StrokeThickness + Size;

            var radiusX = (ActualWidth / 2.0 - offsetX).ClampMin(0);
            var radiusY = (ActualHeight / 2.0 - offsetY).ClampMin(0);

            var x = offsetX + radiusX + radiusX * Math.Sin(Angle * Math.PI / 180.0);
            var y = offsetY + radiusY - radiusY * Math.Cos(Angle * Math.PI / 180.0);

            return new EllipseGeometry(new Point(x, y), Size, Size);
        }
    }
}