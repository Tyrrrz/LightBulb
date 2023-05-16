namespace LightBulb.Core.Utils.Extensions;

public static class NumericExtensions
{
    public static double Wrap(this double value, double min, double max) =>
        value < min
            ? max - (min - value) % (max - min)
            : min + (value - min) % (max - min);
}