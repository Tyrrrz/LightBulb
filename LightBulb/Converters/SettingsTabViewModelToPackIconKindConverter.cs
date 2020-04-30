using System;
using System.Globalization;
using System.Windows.Data;
using LightBulb.ViewModels.Components;
using LightBulb.ViewModels.Components.Settings;
using MaterialDesignThemes.Wpf;

namespace LightBulb.Converters
{
    [ValueConversion(typeof(ISettingsTabViewModel), typeof(PackIconKind))]
    public class SettingsTabViewModelToPackIconKindConverter : IValueConverter
    {
        public static SettingsTabViewModelToPackIconKindConverter Instance { get; } = new SettingsTabViewModelToPackIconKindConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value switch
        {
            GeneralSettingsTabViewModel _ => PackIconKind.Settings,
            LocationSettingsTabViewModel _ => PackIconKind.Globe,
            AdvancedSettingsTabViewModel _ => PackIconKind.CheckboxesMarked,
            ApplicationWhitelistSettingsTabViewModel _ => PackIconKind.Apps,
            HotKeySettingsTabViewModel _ => PackIconKind.Keyboard,
            _ => PackIconKind.QuestionMark // shouldn't happen
        };

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}