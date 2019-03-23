using System;

namespace LightBulb.Models
{
    public class SolarInfo
    {
        public DateTimeOffset Sunrise { get; }

        public DateTimeOffset Sunset { get; }

        public SolarInfo(DateTimeOffset sunrise, DateTimeOffset sunset)
        {
            Sunrise = sunrise;
            Sunset = sunset;
        }

        public override string ToString() => $"Sunrise: {Sunrise:t} / Sunset: {Sunset:t}";
    }
}