using System;
using LightBulb.Models;
using Tyrrrz.Extensions;

namespace LightBulb.Domain
{
    public static class Astronomy
    {
        private static double DegreesToRadians(double degree) => degree * (Math.PI / 180);

        private static double RadiansToDegrees(double radians) => radians * 180 / Math.PI;

        private static double Wrap(double value, double from, double to) =>
            value < from
                ? to - (from - value) % (to - from)
                : from + (value - from) % (to - from);

        private static TimeSpan CalculateSolarTime(GeoLocation location, DateTimeOffset instant, double zenith, bool isSunrise)
        {
            // Based on https://edwilliams.org/sunrise_sunset_algorithm.htm

            // Convert longitude to hour value and calculate an approximate time
            var lngHours = location.Longitude / 15;
            var timeApproxHours = isSunrise ? 6 : 18;
            var timeApproxDays = instant.DayOfYear + (timeApproxHours - lngHours) / 24;

            // Calculate Sun's mean anomaly
            var sunMeanAnomaly = 0.9856 * timeApproxDays - 3.289;

            // Calculate Sun's true longitude
            var sunLng = sunMeanAnomaly + 282.634 +
                         1.916 * Math.Sin(DegreesToRadians(sunMeanAnomaly)) +
                         0.020 * Math.Sin(2 * DegreesToRadians(sunMeanAnomaly));
            sunLng = Wrap(sunLng, 0, 360);

            // Calculate Sun's right ascension
            var sunRightAsc = RadiansToDegrees(Math.Atan(0.91764 * Math.Tan(DegreesToRadians(sunLng))));
            sunRightAsc = Wrap(sunRightAsc, 0, 360);

            // Right ascension value needs to be in the same quadrant as true longitude
            var sunLngQuad = Math.Floor(sunLng / 90) * 90;
            var sunRightAscQuad = Math.Floor(sunRightAsc / 90) * 90;
            var sunRightAscHours = sunRightAsc + (sunLngQuad - sunRightAscQuad);
            sunRightAscHours /= 15;

            // Calculate Sun's declination
            var sinDec = 0.39782 * Math.Sin(DegreesToRadians(sunLng));
            var cosDec = Math.Cos(Math.Asin(sinDec));

            // Calculate Sun's zenith local hour
            var sunLocalHoursCos =
                (Math.Cos(DegreesToRadians(zenith)) - sinDec * Math.Sin(DegreesToRadians(location.Latitude))) /
                (cosDec * Math.Cos(DegreesToRadians(location.Latitude)));

            // This value may be invalid in case the Sun never reaches zenith
            // so we clamp to get the closest highest point instead
            sunLocalHoursCos = sunLocalHoursCos.Clamp(-1, 1);

            // Calculate local time of Sun's highest point
            var sunLocalHours = isSunrise
                ? 360 - RadiansToDegrees(Math.Acos(sunLocalHoursCos))
                : RadiansToDegrees(Math.Acos(sunLocalHoursCos));
            sunLocalHours /= 15;

            // Calculate mean time of the event
            var meanHours = sunLocalHours + sunRightAscHours - 0.06571 * timeApproxDays - 6.622;

            // Adjust mean time to UTC
            var utcHours = meanHours - lngHours;
            utcHours = Wrap(utcHours, 0, 24);

            // Adjust UTC time to local time
            // (we use the offset provided because we can't accurately calculate local timezone from coordinates)
            var localHours = utcHours + instant.Offset.TotalHours;
            localHours = Wrap(localHours, 0, 24);

            return TimeSpan.FromHours(localHours);
        }

        public static TimeSpan CalculateSunriseTime(GeoLocation location, DateTimeOffset instant) =>
            CalculateSolarTime(location, instant, 90.83, true);

        public static TimeSpan CalculateSunsetTime(GeoLocation location, DateTimeOffset instant) =>
            CalculateSolarTime(location, instant, 90.83, false);
    }
}