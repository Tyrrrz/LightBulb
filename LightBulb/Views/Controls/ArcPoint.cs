using System;
using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Media;

namespace LightBulb.Views.Controls;

public class ArcPoint : Shape
{
    public static readonly StyledProperty<double> AngleProperty = AvaloniaProperty.Register<
        ArcPoint,
        double
    >(nameof(Angle), 0.0, false, BindingMode.OneWay, null, (_, baseValue) => baseValue % 360.0);

    public static readonly StyledProperty<double> SizeProperty = AvaloniaProperty.Register<
        ArcPoint,
        double
    >(nameof(Size), 0.0, false, BindingMode.OneWay);

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

    protected override Geometry CreateDefiningGeometry()
    {
        var radiusX = Width / 2.0;
        var radiusY = Height / 2.0;

        var x = radiusX + radiusX * Math.Sin(Angle * Math.PI / 180.0);
        var y = radiusY - radiusY * Math.Cos(Angle * Math.PI / 180.0);

        return new EllipseGeometry(new Rect(new Point(x, y), new Size(Size, Size)));
    }
}
