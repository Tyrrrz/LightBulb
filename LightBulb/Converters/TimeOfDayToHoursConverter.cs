using System;
using System.Globalization;
using System.Windows.Data;
using LightBulb.Domain;

namespace LightBulb.Converters
{
    [ValueConversion(typeof(TimeOfDay), typeof(double))]
    public class TimeOfDayToHoursConverter : IValueConverter
    {
        public static TimeOfDayToHoursConverter Instance { get; } = new TimeOfDayToHoursConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is TimeOfDay timeOfDayValue
                ? timeOfDayValue.AsTimeSpan().TotalHours
                : default;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is double doubleValue
                ? new TimeOfDay(TimeSpan.FromHours(doubleValue))
                : default;
    }
}