using System;
using System.Globalization;
using LightBulb.Domain.Internal;

namespace LightBulb.Domain
{
    public readonly partial struct TimeOfDay
    {
        private readonly TimeSpan _offset;

        public double DayFraction => _offset.TotalDays;

        public TimeOfDay(TimeSpan offset) => _offset = offset;

        public TimeOfDay(DateTimeOffset date) : this(date.TimeOfDay) {}

        public TimeOfDay(int hours, int minutes, int seconds = 0) : this(new TimeSpan(hours, minutes, seconds)) {}

        public TimeOfDay Add(TimeSpan time)
        {
            var ms = (_offset + time).TotalMilliseconds % DayDuration.TotalMilliseconds;
            return new TimeOfDay(TimeSpan.FromMilliseconds(ms));
        }

        public DateTimeOffset NextAfter(DateTimeOffset anchor) =>
            anchor.TimeOfDay <= _offset
                ? anchor.AtTimeOfDay(_offset)
                : anchor.AddDays(1).AtTimeOfDay(_offset);

        public DateTimeOffset Next() => NextAfter(DateTimeOffset.Now);

        public DateTimeOffset PreviousBefore(DateTimeOffset anchor) =>
            anchor.TimeOfDay > _offset
                ? anchor.AtTimeOfDay(_offset)
                : anchor.AddDays(1).AtTimeOfDay(_offset);

        public DateTimeOffset Previous() => PreviousBefore(DateTimeOffset.Now);

        public TimeSpan AsTimeSpan() => _offset;

        public override string ToString() => ToString(null, null);
    }

    public partial struct TimeOfDay
    {
        private static TimeSpan DayDuration { get; } = TimeSpan.FromDays(1);

        public static TimeOfDay? TryParse(string? value, IFormatProvider? formatProvider = null)
        {
            if (DateTimeOffset.TryParse(value, formatProvider, DateTimeStyles.None, out var date))
                return new TimeOfDay(date);

            return null;
        }
    }

    public partial struct TimeOfDay : IEquatable<TimeOfDay>, IComparable<TimeOfDay>, IFormattable
    {
        public bool Equals(TimeOfDay other) => _offset.Equals(other._offset);

        public override bool Equals(object? obj) => obj is TimeOfDay other && Equals(other);

        public int CompareTo(TimeOfDay other) => _offset.CompareTo(other._offset);

        public override int GetHashCode() => _offset.GetHashCode();

        public string ToString(string? format, IFormatProvider? formatProvider) =>
            (DateTimeOffset.Now.ResetTimeOfDay() + _offset).ToString("t", formatProvider);

        public static bool operator ==(TimeOfDay left, TimeOfDay right) => left.Equals(right);

        public static bool operator !=(TimeOfDay left, TimeOfDay right) => !(left == right);

        public static TimeOfDay operator +(TimeOfDay left, TimeSpan right) => left.Add(right);

        public static TimeOfDay operator -(TimeOfDay left, TimeSpan right) => left + right.Negate();
    }
}