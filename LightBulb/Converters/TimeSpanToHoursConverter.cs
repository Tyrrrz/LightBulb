﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace LightBulb.Converters
{
    [ValueConversion(typeof(TimeSpan), typeof(double))]
    public class TimeSpanToHoursConverter : IValueConverter
    {
        public static TimeSpanToHoursConverter Instance { get; } = new TimeSpanToHoursConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is TimeSpan timeSpanValue
                ? timeSpanValue.TotalHours
                : default;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is double doubleValue
                ? TimeSpan.FromHours(doubleValue)
                : default;
    }
}