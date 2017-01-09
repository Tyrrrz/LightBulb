using System;
using NegativeLayer.Extensions;

namespace LightBulb.Services
{
    public class TemperatureService
    {
        /// <summary>
        /// Approximates the appropriate color temperature of indoor lights at the given time
        /// </summary>
        public ushort GetTemperature(TimeSpan time)
        {
            ushort minTemp = Settings.Default.MinTemperature;
            ushort maxTemp = Settings.Default.MaxTemperature;
            int tempDiff = maxTemp - minTemp;

            var offset = Settings.Default.TemperatureSwitchDuration;
            var riseTime = Settings.Default.SunriseTime;
            var setTime = Settings.Default.SunsetTime;

            var pastRiseTime = time - riseTime;
            var pastSetTime = time - setTime;

            // Before sunrise (night)
            if (pastRiseTime <= TimeSpan.Zero)
            {
                double modifier = Math.Abs(pastRiseTime.TotalHours/offset.TotalHours).Clamp(0, 1);
                double temp = minTemp + tempDiff*Math.Cos(modifier*Math.PI/2);
                return (ushort) temp;
            }

            // After sunrise but before sunset (day)
            if (pastSetTime <= TimeSpan.Zero)
                return maxTemp;

            // After sunset (night)
            if (pastSetTime > TimeSpan.Zero)
            {
                double modifier = Math.Abs(pastSetTime.TotalHours/offset.TotalHours).Clamp(0, 1);
                double temp = maxTemp - tempDiff*Math.Sin(modifier*Math.PI/2);
                return (ushort) temp;
            }

            // Unreachable
            return maxTemp;
        }

        /// <summary>
        /// Approximates the appropriate color temperature of indoor lights at the given date
        /// </summary>
        public ushort GetTemperature(DateTime dt) => GetTemperature(dt.TimeOfDay);
    }
}