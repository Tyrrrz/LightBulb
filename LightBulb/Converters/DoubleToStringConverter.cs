﻿using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace LightBulb.Converters;

public class DoubleToStringConverter : IValueConverter
{
    public static DoubleToStringConverter Instance { get; } = new();

    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    ) => value is double doubleValue ? doubleValue.ToString(culture) : default;

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    ) =>
        value is string stringValue
        && double.TryParse(
            stringValue,
            NumberStyles.Float | NumberStyles.AllowThousands,
            culture,
            out var result
        )
            ? result
            : default;
}
