using System;
using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace LightBulb.Views.Controls;

// Modified from an example by Sean Sexton
// https://wpf.2000things.com/2014/09/11/1156-changing-circular-progress-control-to-be-only-an-arc
public class Arc : Shape
{
    public static readonly StyledProperty<double> StartAngleProperty = AvaloniaProperty.Register<
        Arc,
        double
    >(nameof(StartAngle), coerce: (_, a) => a % 360.0);

    public static readonly StyledProperty<double> EndAngleProperty = AvaloniaProperty.Register<
        Arc,
        double
    >(nameof(EndAngle), coerce: (_, a) => a % 360.0);

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

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs args)
    {
        base.OnPropertyChanged(args);

        if (args.Property == StartAngleProperty || args.Property == EndAngleProperty)
            InvalidateGeometry();
    }

    protected override Geometry CreateDefiningGeometry()
    {
        var geometry = new StreamGeometry();
        using var context = geometry.Open();

        var radius = new Size(Width / 2.0, Height / 2.0);

        var start = new Point(
            radius.Width + radius.Width * Math.Sin(StartAngle * Math.PI / 180.0),
            radius.Height - radius.Height * Math.Cos(StartAngle * Math.PI / 180.0)
        );

        var end = new Point(
            radius.Width + radius.Width * Math.Sin(EndAngle * Math.PI / 180.0),
            radius.Height - radius.Height * Math.Cos(EndAngle * Math.PI / 180.0)
        );

        var isLargeArc =
            StartAngle <= EndAngle && Math.Abs(EndAngle - StartAngle) > 180.0
            || StartAngle > EndAngle && Math.Abs(EndAngle - StartAngle) < 180.0;

        context.BeginFigure(start, true);
        context.ArcTo(end, radius, 0.0, isLargeArc, SweepDirection.Clockwise);

        return geometry;
    }
}
