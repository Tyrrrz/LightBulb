using System;
using System.Globalization;
using System.Windows.Data;

namespace LightBulb.Converters;

[ValueConversion(typeof(TimeOnly), typeof(double))]
public class TimeOnlyToHoursConverter : IValueConverter
{
    public static TimeOnlyToHoursConverter Instance { get; } = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is TimeOnly timeOfDayValue ? timeOfDayValue.ToTimeSpan().TotalHours : default;

    public object ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture
    ) =>
        value is double doubleValue
            ? TimeOnly.FromTimeSpan(TimeSpan.FromHours(doubleValue))
            : default;
}
