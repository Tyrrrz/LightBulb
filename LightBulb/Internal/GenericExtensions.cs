using System;
using LightBulb.Domain;
using Tyrrrz.Extensions;

namespace LightBulb.Internal
{
    internal static class GenericExtensions
    {
        public static double StepTo(this double value, double target, double absStep)
        {
            absStep = Math.Abs(absStep);
            return target >= value ? (value + absStep).ClampMax(target) : (value - absStep).ClampMin(target);
        }

        public static DateTimeOffset StepTo(this DateTimeOffset value, DateTimeOffset target, TimeSpan absStep)
        {
            absStep = absStep.Duration();
            return target >= value ? (value + absStep).ClampMax(target) : (value - absStep).ClampMin(target);
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