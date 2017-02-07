using System;

namespace LightBulb.Models
{
    /// <summary>
    /// Information about the sun
    /// </summary>
    public class SolarInfo
    {
        /// <summary>
        /// Time of sunrise
        /// </summary>
        public TimeSpan SunriseTime { get; }

        /// <summary>
        /// Time of sunset
        /// </summary>
        public TimeSpan SunsetTime { get; }

        public SolarInfo(TimeSpan sunriseTime, TimeSpan sunsetTime)
        {
            SunriseTime = sunriseTime;
            SunsetTime = sunsetTime;
        }
    }
}