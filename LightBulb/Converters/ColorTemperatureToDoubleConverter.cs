﻿using System;
using System.Globalization;
using System.Windows.Data;
using LightBulb.Models;

namespace LightBulb.Converters
{
    [ValueConversion(typeof(ColorTemperature), typeof(double))]
    public class ColorTemperatureToDoubleConverter : IValueConverter
    {
        public static ColorTemperatureToDoubleConverter Instance { get; } = new ColorTemperatureToDoubleConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ColorTemperature temperature)
                return temperature.Value;

            throw new ArgumentException("Unexpected value.", nameof(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double temperatureValue)
                return new ColorTemperature(temperatureValue);

            throw new ArgumentException("Unexpected value.", nameof(value));
        }
    }
}