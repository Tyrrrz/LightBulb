using System;

namespace LightBulb.Utils.Extensions;

internal static class DoubleExtensions
{
    extension(double value)
    {
        public double StepTo(double target, double step) =>
            target >= value
                ? Math.Min(value + Math.Abs(step), target)
                : Math.Max(value - Math.Abs(step), target);
    }
}
