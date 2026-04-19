using System;
using LightBulb.Core;

namespace LightBulb.Utils.Extensions;

internal static class ColorConfigurationExtensions
{
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
