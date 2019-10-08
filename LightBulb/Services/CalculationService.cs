using System;
using LightBulb.Helpers;
using LightBulb.Internal;
using LightBulb.Models;

namespace LightBulb.Services
{
    public class CalculationService
    {
        private readonly SettingsService _settingsService;

        public CalculationService(SettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        private TimeSpan GetSunriseTime(DateTimeOffset instant)
        {
            // Short-circuit if manual time is configured or location is not set
            if (_settingsService.IsManualSunriseSunset || _settingsService.Location == null)
                return _settingsService.SunriseTime;

            return Astronomy.CalculateSunrise(_settingsService.Location.Value, instant).TimeOfDay;
        }

        private TimeSpan GetSunsetTime(DateTimeOffset instant)
        {
            // Short-circuit if manual time is configured or location is not set
            if (_settingsService.IsManualSunriseSunset || _settingsService.Location == null)
                return _settingsService.SunsetTime;

            return Astronomy.CalculateSunset(_settingsService.Location.Value, instant).TimeOfDay;
        }

        public ColorTemperature CalculateColorTemperature(DateTimeOffset instant)
        {
            // TODO: transition should end at sunset, not start at sunset

            // Get settings
            var minTemp = _settingsService.MinTemperature;
            var maxTemp = _settingsService.MaxTemperature;
            var offset = _settingsService.TemperatureTransitionDuration;
            var sunriseTime = GetSunriseTime(instant);
            var sunsetTime = GetSunsetTime(instant);

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
                if (instant <= prevSunset + offset)
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
                if (instant <= prevSunrise + offset)
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
    }
}