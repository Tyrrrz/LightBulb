using System;
using System.Globalization;
using System.Windows.Data;
using LightBulb.Internal;

namespace LightBulb.Converters
{
    [ValueConversion(typeof(TimeSpan), typeof(string))]
    public class TimeSpanToTimeOfDayStringConverter : IValueConverter
    {
        public static TimeSpanToTimeOfDayStringConverter Instance { get; } = new TimeSpanToTimeOfDayStringConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan timeSpanValue)
                return (DateTimeOffset.Now.ResetTimeOfDay() + timeSpanValue).ToString("t", culture);

            return default(string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue && DateTimeOffset.TryParse(stringValue, culture, DateTimeStyles.None, out var date))
                return date.TimeOfDay;

            return default(TimeSpan);
        }
    }
}