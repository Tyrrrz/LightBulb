using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace LightBulb.Converters;

public class TimeSpanToHoursDoubleConverter : IValueConverter
{
    public static TimeSpanToHoursDoubleConverter Instance { get; } = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is TimeSpan timeSpanValue ? timeSpanValue.TotalHours : default;

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    ) => value is double doubleValue ? TimeSpan.FromHours(doubleValue) : default;
}
