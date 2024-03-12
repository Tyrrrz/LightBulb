using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace LightBulb.Converters;

public class TimeOnlyToDegreesDoubleConverter : IValueConverter
{
    public static TimeOnlyToDegreesDoubleConverter Instance { get; } = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is TimeOnly timeOfDayValue ? timeOfDayValue.ToTimeSpan().TotalDays * 360.0 : default;

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    ) =>
        value is double doubleValue
            ? TimeOnly.FromTimeSpan(TimeSpan.FromDays(doubleValue / 360.0))
            : default;
}
