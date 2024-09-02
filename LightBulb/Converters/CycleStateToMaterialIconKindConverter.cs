using System;
using System.Globalization;
using Avalonia.Data.Converters;
using LightBulb.Core;
using Material.Icons;

namespace LightBulb.Converters;

public class CycleStateToMaterialIconKindConverter : IValueConverter
{
    public static CycleStateToMaterialIconKindConverter Instance { get; } = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value switch
        {
            CycleState.Disabled => MaterialIconKind.Cancel,
            CycleState.Paused => MaterialIconKind.PauseCircleOutline,
            CycleState.Day => MaterialIconKind.WhiteBalanceSunny,
            CycleState.Night => MaterialIconKind.MoonAndStars,
            CycleState.Transition => MaterialIconKind.Sync,
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
