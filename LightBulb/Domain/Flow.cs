using System;
using LightBulb.Internal;
using LightBulb.Models;

namespace LightBulb.Domain
{
    public static class Flow
    {
        private static double GetCurveValue(
            TimeSpan sunriseStartTime, TimeSpan sunriseEndTime, double dayValue,
            TimeSpan sunsetStartTime, TimeSpan sunsetEndTime, double nightValue,
            DateTimeOffset instant)
        {
            // Between sunrise start and end (transition to day)
            //        /   X   -       -       \
            // -------☀----------------------🌙-------
            //        | trans |       | trans |

            // Assuming it's sunrise, get the time when it started and when it will finish
            var prevSunriseStart = instant.PreviousTimeOfDay(sunriseStartTime);
            var nextSunriseEnd = instant.NextTimeOfDay(sunriseEndTime);

            // Check that it's indeed sunrise.
            // If it's sunrise, previous sunrise end should be before sunrise start (because this one hasn't ended yet).
            if (prevSunriseStart > instant.PreviousTimeOfDay(sunriseEndTime))
            {
                var smoothFactor = (instant - prevSunriseStart) / (nextSunriseEnd - prevSunriseStart);
                return dayValue + (nightValue - dayValue) * Math.Cos(smoothFactor * Math.PI / 2);
            }

            // Between sunset start and end (transition to night)
            //        /       -       -   X   \
            // -------☀----------------------🌙-------
            //        | trans |       | trans |

            // Assuming it's sunset, get the time when it started and when it will finish
            var prevSunsetStart = instant.PreviousTimeOfDay(sunsetStartTime);
            var nextSunsetEnd = instant.NextTimeOfDay(sunsetEndTime);

            // Check that it's indeed sunset.
            // If it's sunset, previous sunset end should be before sunset start (because this one hasn't ended yet).
            if (prevSunsetStart > instant.PreviousTimeOfDay(sunsetEndTime))
            {
                var smoothFactor = (nextSunsetEnd - instant) / (nextSunsetEnd - prevSunsetStart);
                return dayValue + (nightValue - dayValue) * Math.Cos(smoothFactor * Math.PI / 2);
            }

            // Day or night time
            //    X   /       -   X   -       \   X
            // -------☀----------------------🌙-------
            //        | trans |       | trans |

            // Get the upcoming events
            var nextSunriseStart = instant.NextTimeOfDay(sunriseStartTime);
            var nextSunsetStart = instant.NextTimeOfDay(sunsetStartTime);

            // Depending on what event is nearest, it's currently either day or night
            return nextSunriseStart >= nextSunsetStart ? dayValue : nightValue;
        }

        public static ColorConfiguration CalculateColorConfiguration(
            TimeSpan sunriseStartTime, TimeSpan sunriseEndTime, ColorConfiguration dayConfiguration,
            TimeSpan sunsetStartTime, TimeSpan sunsetEndTime, ColorConfiguration nightConfiguration,
            DateTimeOffset instant)
        {
            return new ColorConfiguration(
                GetCurveValue(
                    sunriseStartTime, sunriseEndTime, dayConfiguration.Temperature,
                    sunsetStartTime, sunsetEndTime, nightConfiguration.Temperature,
                    instant),
                GetCurveValue(
                    sunriseStartTime, sunriseEndTime, dayConfiguration.Brightness,
                    sunsetStartTime, sunsetEndTime, nightConfiguration.Brightness,
                    instant)
            );
        }
    }
}