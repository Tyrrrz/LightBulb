using System;
using System.Globalization;
using System.Windows.Data;

namespace LightBulb.Converters
{
    [ValueConversion(typeof(TimeSpan), typeof(double))]
    public class TimeSpanToMinutesConverter : IValueConverter
    {
        public static TimeSpanToMinutesConverter Instance { get; } = new TimeSpanToMinutesConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan timeSpanValue)
                return timeSpanValue.TotalMinutes;

            throw new ArgumentException("Unexpected value.", nameof(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue)
                return TimeSpan.FromMinutes(doubleValue);

            throw new ArgumentException("Unexpected value.", nameof(value));
        }
    }
}