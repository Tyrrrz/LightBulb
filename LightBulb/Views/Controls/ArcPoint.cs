using System;
using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace LightBulb.Views.Controls;

public class ArcPoint : Shape
{
    public static readonly StyledProperty<double> AngleProperty = AvaloniaProperty.Register<
        ArcPoint,
        double
    >(nameof(Angle), coerce: (_, a) => a % 360.0);

    public static readonly StyledProperty<double> SizeProperty = AvaloniaProperty.Register<
        ArcPoint,
        double
    >(nameof(Size));

    public double Angle
    {
        get => GetValue(AngleProperty);
        set => SetValue(AngleProperty, value);
    }

    public double Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs args)
    {
        base.OnPropertyChanged(args);

        if (args.Property == AngleProperty || args.Property == SizeProperty)
            InvalidateGeometry();
    }

    protected override Geometry CreateDefiningGeometry()
    {
        var radius = new Size(Width / 2.0, Height / 2.0);

        var center = new Point(
            radius.Width + radius.Width * Math.Sin(Angle * Math.PI / 180.0) - Size / 2.0,
            radius.Height - radius.Height * Math.Cos(Angle * Math.PI / 180.0) - Size / 2.0
        );

        return new EllipseGeometry(new Rect(center, new Size(Size, Size)));
    }
}
