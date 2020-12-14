using System;

namespace LightBulb.Domain.Internal.Extensions
{
    internal static class DateTimeExtensions
    {
        public static DateTimeOffset ResetTimeOfDay(this DateTimeOffset dateTime) =>
            new(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0, dateTime.Offset);

        public static DateTimeOffset AtTimeOfDay(this DateTimeOffset dateTime, TimeSpan timeOfDay) =>
            dateTime.ResetTimeOfDay() + timeOfDay;
    }
}