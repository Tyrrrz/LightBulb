using System;
using System.Collections.Generic;
using Tyrrrz.Extensions;

namespace LightBulb.Internal
{
    internal static class Extensions
    {
        public static double StepTo(this double value, double target, double absStep)
        {
            absStep = Math.Abs(absStep);
            return target >= value ? (value + absStep).ClampMax(target) : (value - absStep).ClampMin(target);
        }

        public static DateTimeOffset StepTo(this DateTimeOffset value, DateTimeOffset target, TimeSpan absStep)
        {
            absStep = absStep.Duration();
            return target >= value ? (value + absStep).ClampMax(target) : (value - absStep).ClampMin(target);
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
                collection.Add(item);
        }

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

        public static TimeSpan UntilNextMinute(this DateTimeOffset dateTime)
        {
            var lastMinute = new DateTimeOffset(
                dateTime.Year, dateTime.Month, dateTime.Day,
                dateTime.Hour, dateTime.Minute, 0,
                dateTime.Offset);

            return TimeSpan.FromMinutes(1) - (dateTime - lastMinute);
        }

        public static void OpenInBrowser(this Uri uri) => ProcessEx.StartShellExecute(uri.ToString());
    }
}