using System;
using System.Globalization;
using System.Windows.Data;

namespace LightBulb.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBoolConverter : IValueConverter
    {
        public static InverseBoolConverter Instance { get; } = new InverseBoolConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is bool boolValue && !boolValue;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is bool boolValue && !boolValue;
    }
}