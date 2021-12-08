using System;
using System.Globalization;
using System.Windows.Data;

namespace LightBulb.Converters;

[ValueConversion(typeof(TimeSpan), typeof(string))]
public class TimeSpanToDurationStringConverter : IValueConverter
{
    public static TimeSpanToDurationStringConverter Instance { get; } = new();

    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is TimeSpan timeSpanValue
            ? timeSpanValue.ToString("hh\\:mm\\:ss", culture)
            : default;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is string stringValue && TimeSpan.TryParse(stringValue, culture, out var result)
            ? result
            : default;
}