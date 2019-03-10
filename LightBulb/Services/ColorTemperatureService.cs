using System;
using LightBulb.Internal;
using LightBulb.Models;

namespace LightBulb.Services
{
    public class ColorTemperatureService
    {
        private readonly SettingsService _settingsService;

        public ColorTemperatureService(SettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public ColorTemperature GetTemperature(DateTimeOffset instant)
        {
            // TODO: rework

            // Get settings
            var minTemp = _settingsService.MinTemperature;
            var maxTemp = _settingsService.MaxTemperature;
            var offset = _settingsService.TemperatureTransitionDuration;
            var sunriseTime = _settingsService.SunriseTime;
            var sunsetTime = _settingsService.SunsetTime;

            // Get next and previous sunrise and sunset
            var nextSunrise = instant.NextTimeOfDay(sunriseTime);
            var prevSunrise = instant.PreviousTimeOfDay(sunriseTime);
            var nextSunset = instant.NextTimeOfDay(sunsetTime);
            var prevSunset = instant.PreviousTimeOfDay(sunsetTime);

            // Calculate time until next sunrise and sunset
            var untilNextSunrise = nextSunrise - instant;
            var untilNextSunset = nextSunset - instant;

            // Next event is sunrise
            if (untilNextSunrise <= untilNextSunset)
            {
                // Check if in transition period to night
                if (instant <= prevSunset.Add(offset))
                {
                    // Smooth transition
                    var norm = (instant - prevSunset).TotalHours / offset.TotalHours;
                    var value = minTemp.Value + (maxTemp.Value - minTemp.Value) * Math.Cos(norm * Math.PI / 2);
                    return new ColorTemperature(value);
                }

                // Night time
                return minTemp;
            }
            // Next event is sunset
            else
            {
                // Check if in transition period to day
                if (instant <= prevSunrise.Add(offset))
                {
                    // Smooth transition
                    var norm = (instant - prevSunrise).TotalHours / offset.TotalHours;
                    var value = maxTemp.Value + (minTemp.Value - maxTemp.Value) * Math.Cos(norm * Math.PI / 2);
                    return new ColorTemperature(value);
                }

                // Day time
                return maxTemp;
            }
        }

        public ColorTemperature GetTemperature() => GetTemperature(DateTimeOffset.Now);
    }
}