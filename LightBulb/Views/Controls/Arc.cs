using System;
using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Media;

namespace LightBulb.Views.Controls;

// Modified from an example by Sean Sexton
// https://wpf.2000things.com/2014/09/11/1156-changing-circular-progress-control-to-be-only-an-arc
public class Arc : Shape
{
    public static readonly StyledProperty<double> StartAngleProperty = AvaloniaProperty.Register<
        Arc,
        double
    >(
        nameof(StartAngle),
        0.0,
        false,
        BindingMode.OneWay,
        null,
        (_, baseValue) => baseValue % 360.0
    );

    public static readonly StyledProperty<double> EndAngleProperty = AvaloniaProperty.Register<
        Arc,
        double
    >(nameof(EndAngle), 0.0, false, BindingMode.OneWay, null, (_, baseValue) => baseValue % 360.0);

    public double StartAngle
    {
        get => GetValue(StartAngleProperty);
        set => SetValue(StartAngleProperty, value);
    }

    public double EndAngle
    {
        get => GetValue(EndAngleProperty);
        set => SetValue(EndAngleProperty, value);
    }

    protected override Geometry CreateDefiningGeometry()
    {
        var geometry = new StreamGeometry();
        using var ctx = geometry.Open();

        var radiusX = Width / 2.0;
        var radiusY = Height / 2.0;

        var startX = radiusX + radiusX * Math.Sin(StartAngle * Math.PI / 180.0);
        var startY = radiusY - radiusY * Math.Cos(StartAngle * Math.PI / 180.0);

        var endX = radiusX + radiusX * Math.Sin(EndAngle * Math.PI / 180.0);
        var endY = radiusY - radiusY * Math.Cos(EndAngle * Math.PI / 180.0);

        // This single line took me 2 hours to write
        var isLargeArc =
            StartAngle <= EndAngle && Math.Abs(EndAngle - StartAngle) > 180.0
            || StartAngle > EndAngle && Math.Abs(EndAngle - StartAngle) < 180.0;

        ctx.BeginFigure(new Point(startX, startY), true);

        ctx.ArcTo(
            new Point(endX, endY),
            new Size(radiusX, radiusY),
            0.0,
            isLargeArc,
            SweepDirection.Clockwise
        );

        return geometry;
    }
}
