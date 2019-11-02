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
            if (_settingsService.IsManualSunriseSunsetEnabled || _settingsService.Location == null)
                return _settingsService.ManualSunriseTime;

            return Astronomy.CalculateSunrise(_settingsService.Location.Value, instant).TimeOfDay;
        }

        private TimeSpan GetSunsetTime(DateTimeOffset instant)
        {
            // Short-circuit if manual time is configured or location is not set
            if (_settingsService.IsManualSunriseSunsetEnabled || _settingsService.Location == null)
                return _settingsService.ManualSunsetTime;

            return Astronomy.CalculateSunset(_settingsService.Location.Value, instant).TimeOfDay;
        }

        private double GetCurveValue(DateTimeOffset instant, double from, double to)
        {
            // Get settings
            var offset = _settingsService.ConfigurationTransitionDuration;
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
                    return from + (to - from) * Math.Cos(norm * Math.PI / 2);
                }

                // Night time
                return from;
            }
            // Next event is sunset
            else
            {
                // Check if in transition period to day
                if (instant <= prevSunrise + offset)
                {
                    // Smooth transition
                    var norm = (instant - prevSunrise).TotalHours / offset.TotalHours;
                    return to + (from - to) * Math.Cos(norm * Math.PI / 2);
                }

                // Day time
                return to;
            }
        }

        public ColorConfiguration CalculateColorConfiguration(DateTimeOffset instant)
        {
            return new ColorConfiguration(
                GetCurveValue(instant, _settingsService.NightConfiguration.Temperature, _settingsService.DayConfiguration.Temperature),
                GetCurveValue(instant, _settingsService.NightConfiguration.Brightness, _settingsService.DayConfiguration.Brightness));
        }
    }
}