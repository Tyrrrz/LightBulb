using System;
using LightBulb.Core;

namespace LightBulb.Utils.Extensions
{
    internal static class GenericExtensions
    {
        public static double StepTo(this double value, double target, double absStep) =>
            target >= value
                ? Math.Min(value + Math.Abs(absStep), target)
                : Math.Max(value - Math.Abs(absStep), target);

        public static DateTimeOffset StepTo(this DateTimeOffset value, DateTimeOffset target, TimeSpan absStep)
        {
            if (target >= value)
            {
                var result = value + absStep.Duration();
                return result <= target ? result : target;
            }
            else
            {
                var result = value - absStep.Duration();
                return result >= target ? result : target;
            }
        }

        public static ColorConfiguration StepTo(
            this ColorConfiguration value, ColorConfiguration target,
            double temperatureMaxAbsStep, double brightnessMaxAbsStep)
        {
            var temperatureAbsDelta = Math.Abs(target.Temperature - value.Temperature);
            var brightnessAbsDelta = Math.Abs(target.Brightness - value.Brightness);

            var temperatureSteps = temperatureAbsDelta / temperatureMaxAbsStep;
            var brightnessSteps = brightnessAbsDelta / brightnessMaxAbsStep;

            var temperatureAdjustedStep = temperatureSteps >= brightnessSteps
                ? temperatureMaxAbsStep
                : temperatureAbsDelta / brightnessSteps;

            var brightnessAdjustedStep = brightnessSteps >= temperatureSteps
                ? brightnessMaxAbsStep
                : brightnessAbsDelta / temperatureSteps;

            return new ColorConfiguration(
                value.Temperature.StepTo(target.Temperature, temperatureAdjustedStep),
                value.Brightness.StepTo(target.Brightness, brightnessAdjustedStep)
            );
        }
    }
}