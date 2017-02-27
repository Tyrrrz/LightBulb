using System;
using System.Globalization;
using System.Windows.Data;

namespace LightBulb.ViewModels.Converters
{
    [ValueConversion(typeof(TimeSpan), typeof(double))]
    public class TimeSpanToMinutesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ts = (TimeSpan) value;
            return ts.TotalMinutes;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mins = (double) value;
            return TimeSpan.FromMinutes(mins);
        }
    }
}