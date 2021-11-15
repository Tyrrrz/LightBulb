using System;
using System.Globalization;
using System.Windows.Data;

namespace LightBulb.Converters
{
    [ValueConversion(typeof(TimeOnly), typeof(string))]
    public class TimeOnlyToStringConverter : IValueConverter
    {
        public static TimeOnlyToStringConverter Instance { get; } = new();

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is TimeOnly timeOfDay
                ? timeOfDay.ToString(null, culture)
                : default;

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is string stringValue && TimeOnly.TryParse(stringValue, culture, DateTimeStyles.None, out var result)
                ? result
                : default;
    }
}