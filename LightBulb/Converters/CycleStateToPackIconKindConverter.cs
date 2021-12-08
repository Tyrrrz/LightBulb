using System;
using System.Globalization;
using System.Windows.Data;
using LightBulb.Core;
using MaterialDesignThemes.Wpf;

namespace LightBulb.Converters;

[ValueConversion(typeof(CycleState), typeof(PackIconKind))]
public class CycleStateToPackIconKindConverter : IValueConverter
{
    public static CycleStateToPackIconKindConverter Instance { get; } = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value switch
    {
        CycleState.Disabled => PackIconKind.Cancel,
        CycleState.Paused => PackIconKind.PauseCircleOutline,
        CycleState.Day => PackIconKind.WhiteBalanceSunny,
        CycleState.Night => PackIconKind.MoonAndStars,
        CycleState.Transition => PackIconKind.Sync,
        _ => PackIconKind.QuestionMark // shouldn't happen
    };

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}