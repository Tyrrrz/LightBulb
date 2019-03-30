using System;

namespace LightBulb.Internal
{
    internal static class Extensions
    {
        public static DateTimeOffset ResetTimeOfDay(this DateTimeOffset dateTime) =>
            new DateTimeOffset(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0, dateTime.Offset);

        public static DateTimeOffset AtTimeOfDay(this DateTimeOffset dateTime, TimeSpan timeOfDay) =>
            dateTime.ResetTimeOfDay() + timeOfDay;

        public static DateTimeOffset NextTimeOfDay(this DateTimeOffset dateTime, TimeSpan timeOfDay) =>
            dateTime.TimeOfDay <= timeOfDay
                ? dateTime.AtTimeOfDay(timeOfDay)
                : dateTime.AddDays(1).AtTimeOfDay(timeOfDay);

        public static DateTimeOffset PreviousTimeOfDay(this DateTimeOffset dateTime, TimeSpan timeOfDay) =>
            dateTime.TimeOfDay > timeOfDay
                ? dateTime.AtTimeOfDay(timeOfDay)
                : dateTime.AddDays(-1).AtTimeOfDay(timeOfDay);

    }
}