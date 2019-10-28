using System;
using System.Globalization;
using System.Windows.Data;

namespace LightBulb.Converters
{
    [ValueConversion(typeof(double), typeof(double))]
    public class FractionToDegreesConverter : IValueConverter
    {
        public static FractionToDegreesConverter Instance { get; } = new FractionToDegreesConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue)
                return doubleValue * 360.0;

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double doubleValue)
                return doubleValue / 360.0;

            return value;
        }
    }
}