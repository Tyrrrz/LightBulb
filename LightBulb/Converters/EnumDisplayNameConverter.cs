using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Avalonia.Data.Converters;

namespace LightBulb.Converters;

public class EnumDisplayNameConverter : IValueConverter
{
    public static EnumDisplayNameConverter Instance { get; } = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Enum enumValue)
            return default(string);

        var displayName = enumValue
            .GetType()
            .GetMember(enumValue.ToString())
            ?.FirstOrDefault()
            ?.GetCustomAttribute<DisplayAttribute>()
            ?.Name;

        if (!string.IsNullOrWhiteSpace(displayName))
            return displayName;

        return enumValue.ToString();
    }

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    ) => throw new NotSupportedException();
}
