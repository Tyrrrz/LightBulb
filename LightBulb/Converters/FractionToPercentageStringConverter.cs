using System;
using System.Globalization;
using System.Windows.Data;

namespace LightBulb.Converters
{
    [ValueConversion(typeof(double), typeof(string))]
    public class FractionToPercentageStringConverter : IValueConverter
    {
        public static FractionToPercentageStringConverter Instance { get; } = new FractionToPercentageStringConverter();

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue)
                return doubleValue.ToString("P0", culture);

            return default(string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue &&
                double.TryParse(stringValue.Trim('%', ' '), NumberStyles.Float | NumberStyles.AllowThousands, culture, out var result))
            {
                return result / 100.0;
            }

            return default(double);
        }
    }
}