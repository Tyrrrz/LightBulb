using System;
using System.Globalization;
using System.Windows.Data;

namespace LightBulb.Converters
{
    [ValueConversion(typeof(TimeSpan), typeof(string))]
    public class TimeSpanToDurationStringConverter : IValueConverter
    {
        public static TimeSpanToDurationStringConverter Instance { get; } = new TimeSpanToDurationStringConverter();

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan timeSpanValue)
                return timeSpanValue.ToString("hh\\:mm\\:ss", culture);

            return default(string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue && TimeSpan.TryParse(stringValue, culture, out var result))
                return result;

            return default(TimeSpan);
        }
    }
}