using System;
using LightBulb.Internal;
using LightBulb.Models;

namespace LightBulb.Domain
{
    public static class Flow
    {
        private static double GetCurveValue(
            TimeSpan sunriseTime, double dayValue,
            TimeSpan sunsetTime, double nightValue,
            TimeSpan transitionDuration, DateTimeOffset instant)
        {
            // Reflect sunrise & sunset times against the current instant
            var nextSunrise = instant.NextTimeOfDay(sunriseTime);
            var nextSunset = instant.NextTimeOfDay(sunsetTime);

            // Transition to day
            //           x
            // --------------☀----------------------🌙--------------
            //       | trans |                       | trans |
            var sunriseEnd = nextSunrise;
            var sunriseStart = sunriseEnd - transitionDuration;
            if (sunriseStart <= instant && instant <= sunriseEnd)
            {
                var progress = (instant - sunriseStart) / transitionDuration;
                return dayValue + (nightValue - dayValue) * Math.Cos(progress * Math.PI / 2);
            }

            // Transition to night
            //                                           x
            // --------------☀----------------------🌙--------------
            //       | trans |                       | trans |
            var sunsetStart = instant.PreviousTimeOfDay(sunsetTime);
            var sunsetEnd = sunsetStart + transitionDuration;
            if (sunsetStart <= instant && instant <= sunsetEnd)
            {
                var progress = (instant - sunsetStart) / transitionDuration;
                return dayValue + (nightValue - dayValue) * Math.Sin(progress * Math.PI / 2);
            }

            // Day time / night time
            //    x                      x                      x
            // --------------☀----------------------🌙--------------
            //       | trans |                       | trans |
            return nextSunset <= nextSunrise ? dayValue : nightValue;
        }

        public static ColorConfiguration CalculateColorConfiguration(
            TimeSpan sunriseTime, ColorConfiguration dayConfiguration,
            TimeSpan sunsetTime, ColorConfiguration nightConfiguration,
            TimeSpan transitionDuration, DateTimeOffset instant)
        {
            return new ColorConfiguration(
                GetCurveValue(
                    sunriseTime, dayConfiguration.Temperature,
                    sunsetTime, nightConfiguration.Temperature,
                    transitionDuration, instant),
                GetCurveValue(
                    sunriseTime, dayConfiguration.Brightness,
                    sunsetTime, nightConfiguration.Brightness,
                    transitionDuration, instant)
            );
        }
    }
}