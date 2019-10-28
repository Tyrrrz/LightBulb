using System;
using System.Globalization;
using System.Windows.Data;

namespace LightBulb.Converters
{
    [ValueConversion(typeof(double), typeof(string))]
    public class DoubleToStringConverter : IValueConverter
    {
        public static DoubleToStringConverter Instance { get; } = new DoubleToStringConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue)
                return doubleValue.ToString(culture);

            return default(string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue &&
                double.TryParse(stringValue, NumberStyles.Float | NumberStyles.AllowThousands, culture, out var result))
            {
                return result;
            }

            return default(double);
        }
    }
}