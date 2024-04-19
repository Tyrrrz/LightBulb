using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace LightBulb.Converters;

public class TimeSpanToSecondsStringConverter : IValueConverter
{
    public static TimeSpanToSecondsStringConverter Instance { get; } = new();

    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    ) =>
        value is TimeSpan timeSpanValue
            ? timeSpanValue.TotalSeconds.ToString("0.#", culture)
            : default;

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    ) =>
        value is string stringValue && double.TryParse(stringValue, culture, out var result)
            ? TimeSpan.FromSeconds(result)
            : default;
}
