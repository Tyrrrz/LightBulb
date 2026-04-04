using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using LightBulb.PlatformInterop;

namespace LightBulb.Converters;

public class DisplayColorControllerToNameConverter : IValueConverter
{
    public static DisplayColorControllerToNameConverter Instance { get; } = new();

    private static readonly IReadOnlyDictionary<string, string> Names = new Dictionary<
        string,
        string
    >
    {
        ["windows-gdi-native"] = "Windows (native gamma ramp)",
        ["linux-xorg"] = "X.Org / XWayland (xrandr)",
        ["linux-gnome"] = "GNOME Night Light",
        ["linux-plasma"] = "KDE Plasma Night Color",
    };

    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    ) =>
        value is IDisplayColorController controller
        && Names.TryGetValue(controller.Id, out var name)
            ? name
            : value?.ToString();

    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    ) => throw new NotSupportedException();
}
