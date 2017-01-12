using System;
using NegativeLayer.Extensions;

namespace LightBulb.Services
{
    public class TemperatureService
    {
        private double Ease(double from, double to, double t)
        {
            t = t.Clamp(0, 1);
            return from + (to - from)*Math.Sin(t*Math.PI/2);
        }

        /// <summary>
        /// Approximates the appropriate color temperature of indoor lights at the given time
        /// </summary>
        public ushort GetTemperature(TimeSpan time)
        {
            ushort minTemp = Settings.Default.MinTemperature;
            ushort maxTemp = Settings.Default.MaxTemperature;

            var offset = Settings.Default.TemperatureSwitchDuration;
            var halfOffset = TimeSpan.FromHours(offset.TotalHours/2);
            var riseTime = Settings.Default.SunriseTime;
            var setTime = Settings.Default.SunsetTime;
            var riseStartTime = riseTime - halfOffset;
            var riseEndTime = riseTime + halfOffset;
            var setStartTime = setTime - halfOffset;
            var setEndTime = setTime + halfOffset;

            // Before sunrise (night time)
            if (time < riseStartTime)
                return minTemp;

            // Incoming sunrise (night time -> day time)
            if (time >= riseStartTime && time <= riseEndTime)
            {
                double t = (time.TotalHours - riseStartTime.TotalHours)/offset.TotalHours;
                return (ushort) Ease(minTemp, maxTemp, t);
            }

            // Between sunrise and sunset (day time)
            if (time > riseEndTime && time < setStartTime)
                return maxTemp;

            // Incoming sunset (day time -> night time)
            if (time >= setStartTime && time <= setEndTime)
            {
                double t = (time.TotalHours - setStartTime.TotalHours)/offset.TotalHours;
                return (ushort) Ease(maxTemp, minTemp, t);
            }

            // After sunset (night time)
            return minTemp;
        }

        /// <summary>
        /// Approximates the appropriate color temperature of indoor lights at the given date
        /// </summary>
        public ushort GetTemperature(DateTime dt) => GetTemperature(dt.TimeOfDay);
    }
}