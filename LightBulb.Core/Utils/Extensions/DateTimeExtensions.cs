using System;

namespace LightBulb.Core.Utils.Extensions;

public static class DateTimeExtensions
{
    public static TimeOnly ToTimeOnly(this DateTimeOffset dateTime) =>
        TimeOnly.FromTimeSpan(dateTime.TimeOfDay);

    public static DateTimeOffset ResetTimeOfDay(this DateTimeOffset dateTime) =>
        new(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0, dateTime.Offset);

    public static DateTimeOffset AtTimeOfDay(this DateTimeOffset dateTime, TimeSpan timeOfDay) =>
        dateTime.ResetTimeOfDay() + timeOfDay;

    public static DateTimeOffset AtTimeOfDay(this DateTimeOffset dateTime, TimeOnly timeOfDay) =>
        dateTime.AtTimeOfDay(timeOfDay.ToTimeSpan());

    public static DateTimeOffset NextAfter(this TimeOnly time, DateTimeOffset anchor) =>
        anchor.ToTimeOnly() <= time
            ? anchor.AtTimeOfDay(time)
            : anchor.AddDays(1).AtTimeOfDay(time);

    public static DateTimeOffset PreviousBefore(this TimeOnly time, DateTimeOffset anchor) =>
        anchor.ToTimeOnly() > time
            ? anchor.AtTimeOfDay(time)
            : anchor.AddDays(-1).AtTimeOfDay(time);
}
