using System;

namespace LightBulb.Internal
{
    internal static class Extensions
    {
        public static DateTime AtTimeOfDay(this DateTime dateTime, TimeSpan timeOfDay) => dateTime.Date.Add(timeOfDay);

        public static DateTime NextTimeOfDay(this DateTime dateTime, TimeSpan timeOfDay) =>
            dateTime.TimeOfDay <= timeOfDay
                ? dateTime.AtTimeOfDay(timeOfDay)
                : dateTime.AddDays(1).AtTimeOfDay(timeOfDay);

        public static DateTime PreviousTimeOfDay(this DateTime dateTime, TimeSpan timeOfDay) =>
            dateTime.TimeOfDay > timeOfDay
                ? dateTime.AtTimeOfDay(timeOfDay)
                : dateTime.AddDays(-1).AtTimeOfDay(timeOfDay);
    }
}