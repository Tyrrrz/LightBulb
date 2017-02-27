using System;
using System.Globalization;
using System.Windows.Data;

namespace LightBulb.ViewModels.Converters
{
    [ValueConversion(typeof(TimeSpan), typeof(double))]
    public class TimeSpanToHoursConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ts = (TimeSpan) value;
            return ts.TotalHours;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double hours = (double) value;
            return TimeSpan.FromHours(hours);
        }
    }
}