using System;
using LightBulb.Internal;
using LightBulb.Models;

namespace LightBulb.Calculators
{
    public static class Astronomy
    {
        private static DateTimeOffset CalculateSunriseSunset(GeoLocation location, DateTimeOffset instant, bool isSunrise)
        {
            // Based on:
            // https://edwilliams.org/sunrise_sunset_algorithm.htm

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

            // Adjust mean time to UTC
            var hoursUtc = meanTime - lngHours;
            hoursUtc %= 24; // wrap [0;24)

            // Convert back to whichever offset was provided
            return instant.ResetTimeOfDay() + TimeSpan.FromHours(hoursUtc) + instant.Offset;
        }

        public static DateTimeOffset CalculateSunrise(GeoLocation location, DateTimeOffset instant) =>
            CalculateSunriseSunset(location, instant, true);

        public static DateTimeOffset CalculateSunset(GeoLocation location, DateTimeOffset instant) =>
            CalculateSunriseSunset(location, instant, false);
    }
}