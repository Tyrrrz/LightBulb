using System;
using System.Globalization;
using Avalonia.Data.Converters;
using LightBulb.ViewModels.Components.Settings;
using Material.Icons;

namespace LightBulb.Converters;

public class SettingsTabViewModelToMaterialIconKindConverter : IValueConverter
{
    public static SettingsTabViewModelToMaterialIconKindConverter Instance { get; } = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value switch
        {
            GeneralSettingsTabViewModel => MaterialIconKind.Settings,
            LocationSettingsTabViewModel => MaterialIconKind.Globe,
            AdvancedSettingsTabViewModel => MaterialIconKind.CheckboxesMarked,
            ApplicationWhitelistSettingsTabViewModel => MaterialIconKind.Apps,
            HotKeySettingsTabViewModel => MaterialIconKind.Keyboard,
            _ => MaterialIconKind.QuestionMark // shouldn't happen
        };

    public object ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture
    ) => throw new NotSupportedException();
}
