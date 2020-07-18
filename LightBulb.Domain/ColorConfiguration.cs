using System;

namespace LightBulb.Domain
{
    public readonly partial struct ColorConfiguration
    {
        public double Temperature { get; }

        public double Brightness { get; }

        public ColorConfiguration(double temperature, double brightness)
        {
            Temperature = temperature;
            Brightness = brightness;
        }

        public ColorConfiguration WithTemperature(double temperature) =>
            new ColorConfiguration(temperature, Brightness);

        public ColorConfiguration WithBrightness(double brightness) =>
            new ColorConfiguration(Temperature, brightness);

        public override string ToString() => $"{Temperature:F0} K | {Brightness:P0}";
    }

    public partial struct ColorConfiguration
    {
        public static ColorConfiguration Default { get; } = new ColorConfiguration(6600, 1);

        private static double Calculate(
            SolarTimes solarTimes,
            double dayValue,
            double nightValue,
            TimeSpan transitionDuration,
            DateTimeOffset instant)
        {
            // Reflect sunrise & sunset times against the current instant
            var nextSunrise = solarTimes.Sunrise.NextAfter(instant);
            var nextSunset = solarTimes.Sunset.NextAfter(instant);

            // Transition to day
            //           x
            // --------------☀----------------------🌙--------------
            //       | trans |                       | trans |
            var sunriseEnd = nextSunrise;
            var sunriseStart = sunriseEnd - transitionDuration;
            if (sunriseStart <= instant && instant <= sunriseEnd)
            {
                var progress = (instant - sunriseStart) / transitionDuration;
                return dayValue + (nightValue - dayValue) * Math.Cos(progress * Math.PI / 2);
            }

            // Transition to night
            //                                           x
            // --------------☀----------------------🌙--------------
            //       | trans |                       | trans |
            var sunsetStart = solarTimes.Sunset.PreviousBefore(instant);
            var sunsetEnd = sunsetStart + transitionDuration;
            if (sunsetStart <= instant && instant <= sunsetEnd)
            {
                var progress = (instant - sunsetStart) / transitionDuration;
                return dayValue + (nightValue - dayValue) * Math.Sin(progress * Math.PI / 2);
            }

            // Day time / night time
            //    x                      x                      x
            // --------------☀----------------------🌙--------------
            //       | trans |                       | trans |
            return nextSunset <= nextSunrise ? dayValue : nightValue;
        }

        public static ColorConfiguration Calculate(
            SolarTimes solarTimes,
            ColorConfiguration dayConfiguration,
            ColorConfiguration nightConfiguration,
            TimeSpan transitionDuration,
            DateTimeOffset instant)
        {
            return new ColorConfiguration(
                Calculate(
                    solarTimes,
                    dayConfiguration.Temperature,
                    nightConfiguration.Temperature,
                    transitionDuration,
                    instant),
                Calculate(
                    solarTimes,
                    dayConfiguration.Brightness,
                    nightConfiguration.Brightness,
                    transitionDuration,
                    instant)
            );
        }
    }

    public partial struct ColorConfiguration : IEquatable<ColorConfiguration>
    {
        public bool Equals(ColorConfiguration other) => Temperature.Equals(other.Temperature) && Brightness.Equals(other.Brightness);

        public override bool Equals(object? obj) => obj is ColorConfiguration other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Temperature, Brightness);

        public static bool operator ==(ColorConfiguration a, ColorConfiguration b) => a.Equals(b);

        public static bool operator !=(ColorConfiguration a, ColorConfiguration b) => !(a == b);
    }
}