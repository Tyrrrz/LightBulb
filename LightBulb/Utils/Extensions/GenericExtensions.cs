using System;
using LightBulb.Core;

namespace LightBulb.Utils.Extensions;

internal static class GenericExtensions
{
    public static double StepTo(this double value, double target, double step) =>
        target >= value
            ? Math.Min(value + Math.Abs(step), target)
            : Math.Max(value - Math.Abs(step), target);

    public static DateTimeOffset StepTo(
        this DateTimeOffset value,
        DateTimeOffset target,
        TimeSpan step
    )
    {
        if (target >= value)
        {
            var result = value + step.Duration();
            return result <= target ? result : target;
        }
        else
        {
            var result = value - step.Duration();
            return result >= target ? result : target;
        }
    }

    public static ColorConfiguration StepTo(
        this ColorConfiguration value,
        ColorConfiguration target,
        double temperatureMaxStep,
        double brightnessMaxStep
    )
    {
        var temperatureDelta = Math.Abs(target.Temperature - value.Temperature);
        var brightnessDelta = Math.Abs(target.Brightness - value.Brightness);

        var temperatureSteps = temperatureDelta / temperatureMaxStep;
        var brightnessSteps = brightnessDelta / brightnessMaxStep;

        var temperatureAdjustedStep =
            temperatureSteps >= brightnessSteps
                ? temperatureMaxStep
                : temperatureDelta / brightnessSteps;

        var brightnessAdjustedStep =
            brightnessSteps >= temperatureSteps
                ? brightnessMaxStep
                : brightnessDelta / temperatureSteps;

        return new ColorConfiguration(
            value.Temperature.StepTo(target.Temperature, temperatureAdjustedStep),
            value.Brightness.StepTo(target.Brightness, brightnessAdjustedStep)
        );
    }
}
