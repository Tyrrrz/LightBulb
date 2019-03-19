using System;
using System.Globalization;
using System.Windows.Data;

namespace LightBulb.Converters
{
    [ValueConversion(typeof(TimeSpan), typeof(double))]
    public class TimeSpanToHoursConverter : IValueConverter
    {
        public static TimeSpanToHoursConverter Instance { get; } = new TimeSpanToHoursConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan timeSpanValue)
                return timeSpanValue.TotalHours;

            throw new ArgumentException("Unexpected value.", nameof(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue)
                return TimeSpan.FromHours(doubleValue);

            throw new ArgumentException("Unexpected value.", nameof(value));
        }
    }
}