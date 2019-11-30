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
                if (instant <= prevSunset + transitionDuration)
                {
                    // Smooth transition
                    var norm = (instant - prevSunset) / transitionDuration;
                    return nightValue + (dayValue - nightValue) * Math.Cos(norm * Math.PI / 2);
                }

                // Night time
                return nightValue;
            }
            // Next event is sunset
            else
            {
                // Check if in transition period to day
                if (instant <= prevSunrise + transitionDuration)
                {
                    // Smooth transition
                    var norm = (instant - prevSunrise) / transitionDuration;
                    return dayValue + (nightValue - dayValue) * Math.Cos(norm * Math.PI / 2);
                }

                // Day time
                return dayValue;
            }
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