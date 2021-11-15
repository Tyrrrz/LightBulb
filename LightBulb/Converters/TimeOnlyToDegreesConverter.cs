using System;
using System.Globalization;
using System.Windows.Data;

namespace LightBulb.Converters
{
    [ValueConversion(typeof(TimeOnly), typeof(double))]
    public class TimeOnlyToDegreesConverter : IValueConverter
    {
        public static TimeOnlyToDegreesConverter Instance { get; } = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is TimeOnly timeOfDayValue
                ? timeOfDayValue.ToTimeSpan().TotalDays * 360.0
                : default;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is double doubleValue
                ? TimeOnly.FromTimeSpan(TimeSpan.FromDays(doubleValue / 360.0))
                : default;
    }
}