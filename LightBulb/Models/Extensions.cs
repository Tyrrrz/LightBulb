using System;
using LightBulb.Internal;
using LightBulb.WindowsApi.Models;

namespace LightBulb.Models
{
    public static class Extensions
    {
        public static ColorConfiguration StepTo(this ColorConfiguration value, ColorConfiguration target,
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

        public static ColorBalance ToColorBalance(this ColorConfiguration colorConfiguration)
        {
            // Algorithm taken from http://www.tannerhelland.com/4435/convert-temperature-rgb-algorithm-code/

            double GetRed()
            {
                if (colorConfiguration.Temperature > 6600)
                    return Math.Pow(colorConfiguration.Temperature / 100 - 60, -0.1332047592) * 329.698727446 / 255;

                return 1;
            }

            double GetGreen()
            {
                if (colorConfiguration.Temperature > 6600)
                    return Math.Pow(colorConfiguration.Temperature / 100 - 60, -0.0755148492) * 288.1221695283 / 255;

                return (Math.Log(colorConfiguration.Temperature / 100) * 99.4708025861 - 161.1195681661) / 255;
            }

            double GetBlue()
            {
                if (colorConfiguration.Temperature >= 6600)
                    return 1;

                if (colorConfiguration.Temperature <= 1900)
                    return 0;

                return (Math.Log(colorConfiguration.Temperature / 100 - 10) * 138.5177312231 - 305.0447927307) / 255;
            }

            return new ColorBalance(
                GetRed() * colorConfiguration.Brightness,
                GetGreen() * colorConfiguration.Brightness,
                GetBlue() * colorConfiguration.Brightness
            );
        }
    }
}