using System;
using LightBulb.Internal;
using LightBulb.Models;

namespace LightBulb.Logic
{
    public static class FlowLogic
    {
        private static double GetCurveValue(
            TimeSpan sunriseTime, double dayValue,
            TimeSpan sunsetTime, double nightValue,
            TimeSpan transitionDuration, DateTimeOffset instant)
        {
            // Reflect sunrise & sunset times against the current instant
            var nextSunrise = instant.NextTimeOfDay(sunriseTime);
            var prevSunrise = instant.PreviousTimeOfDay(sunriseTime);
            var nextSunset = instant.NextTimeOfDay(sunsetTime);

            // After sunrise (transition to day)
            //        |   X   |       |       |
            // -------☀----------------------🌙-------
            //        | trans |       | trans |
            if (instant >= prevSunrise && instant <= prevSunrise + transitionDuration)
            {
                var smoothFactor = (instant - prevSunrise) / transitionDuration;
                return dayValue + (nightValue - dayValue) * Math.Cos(smoothFactor * Math.PI / 2);
            }

            // Before sunset (transition to night)
            //        |       |       |   X   |
            // -------☀----------------------🌙-------
            //        | trans |       | trans |
            if (instant >= nextSunset - transitionDuration && instant <= nextSunset)
            {
                var smoothFactor = (nextSunset - instant) / transitionDuration;
                return dayValue + (nightValue - dayValue) * Math.Cos(smoothFactor * Math.PI / 2);
            }

            // Between sunrise and sunset
            //    X   |       |   X   |       |   X
            // -------☀----------------------🌙-------
            //        | trans |       | trans |
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