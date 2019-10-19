using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using LightBulb.Internal;

namespace LightBulb.Converters
{
    [ValueConversion(typeof(TimeSpan), typeof(string))]
    public class TimeSpanToHumanizedDurationStringConverter : IValueConverter
    {
        public static TimeSpanToHumanizedDurationStringConverter Instance { get; } = new TimeSpanToHumanizedDurationStringConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan timeSpanValue)
            {
                if (timeSpanValue.TotalMinutes < 1)
                    return $"{timeSpanValue.Seconds} {(timeSpanValue.Seconds == 1 ? "second" : "seconds")}";

                var buffer = new StringBuilder();

                if (timeSpanValue.Hours > 0)
                {
                    buffer.Append(timeSpanValue.Hours);
                    buffer.Append(' ');
                    buffer.Append(timeSpanValue.Hours == 1 ? "hour" : "hours");
                }

                if (timeSpanValue.Minutes > 0)
                {
                    buffer.AppendIfNotEmpty(' ');
                    buffer.Append(timeSpanValue.Minutes);
                    buffer.Append(' ');
                    buffer.Append(timeSpanValue.Minutes == 1 ? "minute" : "minutes");
                }

                return buffer.ToString();

            }

            return default(string);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}