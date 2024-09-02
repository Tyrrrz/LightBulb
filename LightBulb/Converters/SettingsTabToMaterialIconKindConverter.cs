using System;
using System.Globalization;
using Avalonia.Data.Converters;
using LightBulb.ViewModels.Components.Settings;
using Material.Icons;

namespace LightBulb.Converters;

public class SettingsTabToMaterialIconKindConverter : IValueConverter
{
    public static SettingsTabToMaterialIconKindConverter Instance { get; } = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value switch
        {
            GeneralSettingsTabViewModel => MaterialIconKind.Settings,
            LocationSettingsTabViewModel => MaterialIconKind.Globe,
            AdvancedSettingsTabViewModel => MaterialIconKind.CheckboxesMarked,
            ApplicationWhitelistSettingsTabViewModel => MaterialIconKind.Apps,
            HotKeySettingsTabViewModel => MaterialIconKind.Keyboard,
            // Shouldn't happen
            _ => MaterialIconKind.QuestionMark,
        };

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    ) => throw new NotSupportedException();
}
