using System;
using LightBulb.Core;

namespace LightBulb.Utils.Extensions;

internal static class GenericExtensions
{
    extension(double value)
    {
        public double StepTo(double target, double step) =>
            target >= value
                ? Math.Min(value + Math.Abs(step), target)
                : Math.Max(value - Math.Abs(step), target);
    }

    extension(DateTimeOffset value)
    {
        public DateTimeOffset StepTo(DateTimeOffset target, TimeSpan step)
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
    }

    extension(ColorConfiguration value)
    {
        public ColorConfiguration StepTo(
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
}
