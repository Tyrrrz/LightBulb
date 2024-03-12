using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace LightBulb.Converters;

public class TimeOnlyToHoursDoubleConverter : IValueConverter
{
    public static TimeOnlyToHoursDoubleConverter Instance { get; } = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is TimeOnly timeOfDayValue ? timeOfDayValue.ToTimeSpan().TotalHours : default;

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    ) =>
        value is double doubleValue
            ? TimeOnly.FromTimeSpan(TimeSpan.FromHours(doubleValue))
            : default;
}
