using System;
using System.Globalization;
using System.Windows.Data;
using LightBulb.Models;

namespace LightBulb.Converters
{
    [ValueConversion(typeof(ColorTemperature), typeof(string))]
    public class ColorTemperatureToStringConverter : IValueConverter
    {
        public static ColorTemperatureToStringConverter Instance { get; } = new ColorTemperatureToStringConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ColorTemperature temperature)
                return $"{temperature.Value}K";

            return default(string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                var stringValueCleaned = stringValue.Trim('K', 'k', ' ');
                return new ColorTemperature(double.Parse(stringValueCleaned, CultureInfo.InvariantCulture));
            }

            return default(ColorTemperature);
        }
    }
}