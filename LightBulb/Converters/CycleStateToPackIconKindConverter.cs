using System;
using System.Globalization;
using System.Windows.Data;
using LightBulb.Models;
using MaterialDesignThemes.Wpf;

namespace LightBulb.Converters
{
    [ValueConversion(typeof(CycleState), typeof(PackIconKind))]
    public class CycleStateToPackIconKindConverter : IValueConverter
    {
        public static CycleStateToPackIconKindConverter Instance { get; } = new CycleStateToPackIconKindConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CycleState cycleStateValue)
            {
                return cycleStateValue switch
                {
                    CycleState.Disabled => PackIconKind.Cancel,
                    CycleState.Paused => PackIconKind.PauseCircleOutline,
                    CycleState.Day => PackIconKind.WhiteBalanceSunny,
                    CycleState.Night => PackIconKind.MoonAndStars,
                    CycleState.Transition => PackIconKind.Sync,
                    _ => PackIconKind.QuestionMark // shouldn't happen
                };
            }

            return PackIconKind.QuestionMark;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}