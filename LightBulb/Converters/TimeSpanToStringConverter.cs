using System;
using System.Globalization;
using System.Windows.Data;

namespace LightBulb.Converters
{
    [ValueConversion(typeof(TimeSpan), typeof(double))]
    public class TimeSpanToStringConverter : IValueConverter
    {
        private const string DefaultFormat = "hh\\:mm\\:ss";

        public static TimeSpanToStringConverter Instance { get; } = new TimeSpanToStringConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var format = parameter as string ?? DefaultFormat;

            if (value is TimeSpan timeSpanValue)
                return timeSpanValue.ToString(format, CultureInfo.InvariantCulture);

            return default(string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var format = parameter as string ?? DefaultFormat;

            if (value is string stringValue &&
                TimeSpan.TryParseExact(stringValue, format, CultureInfo.InvariantCulture, out var timeSpanValue))
            {
                return timeSpanValue;
            }

            return default(TimeSpan);
        }
    }
}