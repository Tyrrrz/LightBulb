using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace LightBulb.Converters;

public class TimeSpanToSecondsDoubleConverter : IValueConverter
{
    public static TimeSpanToSecondsDoubleConverter Instance { get; } = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is TimeSpan timeSpanValue ? timeSpanValue.TotalSeconds : default;

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    ) => value is double doubleValue ? TimeSpan.FromSeconds(doubleValue) : default;
}
