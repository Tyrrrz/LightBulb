using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LightBulb.Views.Controls;

public class ArcPoint : Shape
{
    private static object CoerceAngle(DependencyObject d, object baseValue) =>
        baseValue is double angle ? angle % 360.0 : baseValue;

    public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
        nameof(Angle),
        typeof(double),
        typeof(ArcPoint),
        new FrameworkPropertyMetadata(
            0.0,
            FrameworkPropertyMetadataOptions.AffectsRender,
            null,
            CoerceAngle
        )
    );

    public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(
        nameof(Size),
        typeof(double),
        typeof(ArcPoint),
        new FrameworkPropertyMetadata(
            2.0,
            FrameworkPropertyMetadataOptions.AffectsRender,
            null,
            CoerceAngle
        )
    );

    public double Angle
    {
        get => (double)GetValue(AngleProperty);
        set => SetValue(AngleProperty, value);
    }

    public double Size
    {
        get => (double)GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    protected override Geometry DefiningGeometry
    {
        get
        {
            var radiusX = ActualWidth / 2.0;
            var radiusY = ActualHeight / 2.0;

            var x = radiusX + radiusX * Math.Sin(Angle * Math.PI / 180.0);
            var y = radiusY - radiusY * Math.Cos(Angle * Math.PI / 180.0);

            return new EllipseGeometry(new Point(x, y), Size, Size);
        }
    }
}
