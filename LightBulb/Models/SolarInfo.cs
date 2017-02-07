using System;

namespace LightBulb.Models
{
    public class SolarInfo
    {
        public TimeSpan SunriseTime { get; }

        public TimeSpan SunsetTime { get; }

        public SolarInfo(TimeSpan sunriseTime, TimeSpan sunsetTime)
        {
            SunriseTime = sunriseTime;
            SunsetTime = sunsetTime;
        }
    }
}