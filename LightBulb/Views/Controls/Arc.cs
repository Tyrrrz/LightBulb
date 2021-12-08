using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LightBulb.Views.Controls;
// Modified from an example by Sean Sexton
// https://wpf.2000things.com/2014/09/11/1156-changing-circular-progress-control-to-be-only-an-arc/

public sealed class Arc : Shape
{
    private static object CoerceAngle(DependencyObject d, object baseValue) =>
        baseValue is double angle
            ? angle % 360.0
            : baseValue;

    public static readonly DependencyProperty StartAngleProperty = DependencyProperty.Register(
        nameof(StartAngle),
        typeof(double),
        typeof(Arc),
        new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender, null, CoerceAngle)
    );

    public static readonly DependencyProperty EndAngleProperty = DependencyProperty.Register(
        nameof(EndAngle),
        typeof(double),
        typeof(Arc),
        new FrameworkPropertyMetadata(90.0, FrameworkPropertyMetadataOptions.AffectsRender, null, CoerceAngle)
    );

    public double StartAngle
    {
        get => (double) GetValue(StartAngleProperty);
        set => SetValue(StartAngleProperty, value);
    }

    public double EndAngle
    {
        get => (double) GetValue(EndAngleProperty);
        set => SetValue(EndAngleProperty, value);
    }

    protected override Geometry DefiningGeometry
    {
        get
        {
            var geometry = new StreamGeometry();
            using var ctx = geometry.Open();

            var offsetX = StrokeThickness / 2.0;
            var offsetY = StrokeThickness / 2.0;

            var radiusX = Math.Max(ActualWidth / 2.0 - offsetX, 0);
            var radiusY = Math.Max(ActualHeight / 2.0 - offsetY, 0);

            var startX = offsetX + radiusX + radiusX * Math.Sin(StartAngle * Math.PI / 180.0);
            var startY = offsetY + radiusY - radiusY * Math.Cos(StartAngle * Math.PI / 180.0);

            var endX = offsetX + radiusX + radiusX * Math.Sin(EndAngle * Math.PI / 180.0);
            var endY = offsetY + radiusY - radiusY * Math.Cos(EndAngle * Math.PI / 180.0);

            // This single line took me 2 hours to write
            var isLargeArc =
                StartAngle <= EndAngle && Math.Abs(EndAngle - StartAngle) > 180.0 ||
                StartAngle > EndAngle && Math.Abs(EndAngle - StartAngle) < 180.0;

            ctx.BeginFigure(
                new Point(startX, startY),
                true,
                false
            );

            ctx.ArcTo(
                new Point(endX, endY),
                new Size(radiusX, radiusY),
                0.0,
                isLargeArc,
                SweepDirection.Clockwise,
                true,
                false
            );

            return geometry;
        }
    }
}