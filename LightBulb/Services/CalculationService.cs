using System;
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

        public ColorTemperature CalculateColorTemperature(DateTimeOffset instant)
        {
            // TODO: transition should end at sunset, not start at sunset

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

        private DateTimeOffset CalculateSunriseSunset(GeoLocation location, DateTimeOffset instant,
            bool isSunrise)
        {
            // Based on:
            // https://github.com/ceeK/Solar/blob/9d8ed80a3977c97d7a2014ef28b129ec80c52a70/Solar/Solar.swift
            // Copyright (c) 2016 Chris Howell (MIT License)

            // TODO: this is not 100% accurate

            // Convert longitude to hour value and calculate an approximate time
            var lngHours = location.Longitude / 15;
            var timeApproxHours = isSunrise ? 6 : 18;
            var timeApproxDays = instant.DayOfYear + (timeApproxHours - lngHours) / 24;

            // Calculate the Sun's mean anomaly
            var sunMeanAnomaly = 0.9856 * timeApproxDays - 3.289;

            // Calculate the Sun's true longitude
            var sunLng = sunMeanAnomaly + 282.634 +
                         1.916 * Math.Sin(UnitConversion.DegreesToRadians(sunMeanAnomaly)) +
                         0.020 * Math.Sin(2 * UnitConversion.DegreesToRadians(sunMeanAnomaly));

            sunLng %= 360; // wrap [0;360)

            // Calculate the Sun's right ascension
            var sunRightAsc = UnitConversion.RadiansToDegrees(Math.Atan(0.91764 * Math.Tan(UnitConversion.DegreesToRadians(sunLng))));
            sunRightAsc %= 360; // wrap [0;360)

            // Right ascension value needs to be in the same quadrant as true longitude
            var sunLngQuad = Math.Floor(sunLng / 90) * 90;
            var sunRightAscQuad = Math.Floor(sunRightAsc / 90) * 90;
            var sunRightAscHours = sunRightAsc + (sunLngQuad - sunRightAscQuad);
            sunRightAscHours /= 15;

            // Calculate Sun's declination
            var sinDec = 0.39782 * Math.Sin(UnitConversion.DegreesToRadians(sunLng));
            var cosDec = Math.Cos(Math.Asin(sinDec));

            // Calculate the Sun's local hour angle
            const double zenith = 90.83; // official sunrise/sunset
            var sunLocalHoursCos =
                (Math.Cos(UnitConversion.DegreesToRadians(zenith)) - sinDec * Math.Sin(UnitConversion.DegreesToRadians(location.Latitude))) /
                (cosDec * Math.Cos(UnitConversion.DegreesToRadians(location.Latitude)));
            var sunLocalHours = isSunrise
                ? 360 - UnitConversion.RadiansToDegrees(Math.Acos(sunLocalHoursCos))
                : UnitConversion.RadiansToDegrees(Math.Acos(sunLocalHoursCos));
            sunLocalHours /= 15;

            // Calculate local mean time
            var meanTime = sunLocalHours + sunRightAscHours - 0.06571 * timeApproxDays - 6.622;

            return instant.ResetTimeOfDay() + TimeSpan.FromHours(meanTime);
        }

        public DateTimeOffset CalculateSunrise(GeoLocation location, DateTimeOffset instant) =>
            CalculateSunriseSunset(location, instant, true);

        public DateTimeOffset CalculateSunset(GeoLocation location, DateTimeOffset instant) =>
            CalculateSunriseSunset(location, instant, false);
    }
}