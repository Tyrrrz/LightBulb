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

            // Way before sunrise (night time)
            if (time < riseTime - offset)
                return minTemp;

            // Incoming sunrise (night time -> day time)
            if (time >= riseTime - offset && time < riseTime)
            {
                double remaining = Math.Abs(time.TotalHours - riseTime.TotalHours);
                double modifier = (remaining/offset.TotalHours).Clamp(0, 1);
                double temp = minTemp + tempDiff*Math.Cos(modifier*Math.PI/2);
                return (ushort) temp;
            }

            // Between sunrise and sunset (day time)
            if (time > riseTime && time < setTime - offset)
                return maxTemp;

            // Incoming sunset (day time -> night time)
            if (time >= setTime - offset && time < setTime)
            {
                double remaining = Math.Abs(time.TotalHours - setTime.TotalHours);
                double modifier = (remaining/offset.TotalHours).Clamp(0, 1);
                double temp = maxTemp - tempDiff*Math.Cos(modifier*Math.PI/2);
                return (ushort) temp;
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