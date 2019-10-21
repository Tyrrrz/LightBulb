using System;
using LightBulb.WindowsApi.Models;
using Tyrrrz.Extensions;

namespace LightBulb.Models
{
    public static class Extensions
    {
        public static double Interpolate(this double from, double to, double absStep)
        {
            absStep = Math.Abs(absStep);

            return to >= from ? (from + absStep).ClampMax(to) : (from - absStep).ClampMin(to);
        }

        public static ColorConfiguration Interpolate(this ColorConfiguration from, ColorConfiguration to)
        {
            var temperatureAbsDelta = Math.Abs(to.Temperature - from.Temperature);
            var brightnessAbsDelta = Math.Abs(to.Brightness - from.Brightness);

            // Adjust brightness step so that both properties finish interpolating at the same time
            const double temperatureAbsStep = 30.0;
            var brightnessAbsStep = temperatureAbsDelta > 0 ? brightnessAbsDelta * temperatureAbsStep / temperatureAbsDelta : 0.008;

            // Interpolate
            var resultTemperature = from.Temperature.Interpolate(to.Temperature, temperatureAbsStep);
            var resultBrightness = from.Brightness.Interpolate(to.Brightness, brightnessAbsStep);

            return new ColorConfiguration(resultTemperature, resultBrightness);
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