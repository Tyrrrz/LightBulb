using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace LightBulb.Converters;

public class TimeOnlyToStringConverter : IValueConverter
{
    public static TimeOnlyToStringConverter Instance { get; } = new();

    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    ) => value is TimeOnly timeOfDay ? timeOfDay.ToString(null, culture) : default;

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    ) =>
        value is string stringValue
        && TimeOnly.TryParse(stringValue, culture, DateTimeStyles.None, out var result)
            ? result
            : default;
}
