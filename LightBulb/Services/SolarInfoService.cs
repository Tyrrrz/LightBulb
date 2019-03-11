using System;
using LightBulb.Models;

namespace LightBulb.Services
{
    public class SolarInfoService
    {
        private double DegreesToRadians(double degree) => degree * (Math.PI / 180);

        private double RadiansToDegrees(double radians) => radians * 180 / Math.PI;

        private DateTimeOffset GetEventTime(GeographicCoordinates location, DateTimeOffset day,
            bool isSunrise)
        {
            // Original code credit:
            // https://github.com/ceeK/Solar/blob/9d8ed80a3977c97d7a2014ef28b129ec80c52a70/Solar/Solar.swift
            // Copyright (c) 2016 Chris Howell (MIT License)

            // Convert longitude to hour value and calculate an approximate time
            var lngHours = location.Longitude / 15;
            var timeApproxHours = isSunrise ? 6 : 18;
            var timeApproxDays = day.DayOfYear + (timeApproxHours - lngHours) / 24;

            // Calculate the Sun's mean anomaly
            var sunMeanAnomaly = 0.9856 * timeApproxDays - 3.289;

            // Calculate the Sun's true longitude
            var sunLng = sunMeanAnomaly + 282.634 +
                         1.916 * Math.Sin(DegreesToRadians(sunMeanAnomaly)) +
                         0.020 * Math.Sin(2 * DegreesToRadians(sunMeanAnomaly));
            sunLng = sunLng % 360; // wrap [0;360)

            // Calculate the Sun's right ascension
            var sunRightAsc = RadiansToDegrees(Math.Atan(0.91764 * Math.Tan(DegreesToRadians(sunLng))));
            sunRightAsc = sunRightAsc % 360; // wrap [0;360)

            // Right ascension value needs to be in the same quadrant as true longitude
            var sunLngQuad = Math.Floor(sunLng / 90) * 90;
            var sunRightAscQuad = Math.Floor(sunRightAsc / 90) * 90;
            var sunRightAscHours = sunRightAsc + (sunLngQuad - sunRightAscQuad);
            sunRightAscHours = sunRightAscHours / 15;

            // Calculate Sun's declination
            var sinDec = 0.39782 * Math.Sin(DegreesToRadians(sunLng));
            var cosDec = Math.Cos(Math.Asin(sinDec));

            // Calculate the Sun's local hour angle
            const double zenith = 90.83; // official sunrise/sunset
            var sunLocalHoursCos =
                (Math.Cos(DegreesToRadians(zenith)) - sinDec * Math.Sin(DegreesToRadians(location.Latitude))) /
                (cosDec * Math.Cos(DegreesToRadians(location.Latitude)));
            var sunLocalHours = isSunrise
                ? 360 - RadiansToDegrees(Math.Acos(sunLocalHoursCos))
                : RadiansToDegrees(Math.Acos(sunLocalHoursCos));
            sunLocalHours = sunLocalHours / 15;

            // Calculate local mean time
            var meanTime = sunLocalHours + sunRightAscHours - 0.06571 * timeApproxDays - 6.622;

            return day.Date.AddHours(meanTime);
        }

        public SolarInfo GetSolarInfo(GeographicCoordinates location, DateTimeOffset day)
        {
            var sunrise = GetEventTime(location, day, true);
            var sunset = GetEventTime(location, day, false);

            return new SolarInfo(sunrise, sunset);
        }

        public SolarInfo GetSolarInfo(GeographicCoordinates location) => GetSolarInfo(location, DateTimeOffset.Now);
    }
}