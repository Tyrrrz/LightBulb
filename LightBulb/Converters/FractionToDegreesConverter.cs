﻿using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace LightBulb.Converters;

public class FractionToDegreesConverter : IValueConverter
{
    public static FractionToDegreesConverter Instance { get; } = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is double doubleValue ? doubleValue * 360.0 : default;

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    ) => value is double doubleValue ? doubleValue / 360.0 : default;
}
