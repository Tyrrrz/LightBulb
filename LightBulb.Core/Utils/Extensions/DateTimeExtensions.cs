using System;

namespace LightBulb.Core.Utils.Extensions;

public static class DateTimeExtensions
{
    extension(DateTimeOffset dateTime)
    {
        public TimeOnly ToTimeOnly() => TimeOnly.FromTimeSpan(dateTime.TimeOfDay);

        public DateTimeOffset ResetTimeOfDay() =>
            new(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0, dateTime.Offset);

        public DateTimeOffset AtTimeOfDay(TimeSpan timeOfDay) =>
            dateTime.ResetTimeOfDay() + timeOfDay;

        public DateTimeOffset AtTimeOfDay(TimeOnly timeOfDay) =>
            dateTime.AtTimeOfDay(timeOfDay.ToTimeSpan());
    }

    extension(TimeOnly time)
    {
        public DateTimeOffset NextAfter(DateTimeOffset anchor) =>
            anchor.ToTimeOnly() <= time
                ? anchor.AtTimeOfDay(time)
                : anchor.AddDays(1).AtTimeOfDay(time);

        public DateTimeOffset PreviousBefore(DateTimeOffset anchor) =>
            anchor.ToTimeOnly() > time
                ? anchor.AtTimeOfDay(time)
                : anchor.AddDays(-1).AtTimeOfDay(time);
    }
}
