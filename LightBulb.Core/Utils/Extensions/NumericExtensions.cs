namespace LightBulb.Core.Utils.Extensions;

public static class NumericExtensions
{
    extension(double value)
    {
        public double Wrap(double min, double max) =>
            value < min ? max - (min - value) % (max - min) : min + (value - min) % (max - min);
    }
}
