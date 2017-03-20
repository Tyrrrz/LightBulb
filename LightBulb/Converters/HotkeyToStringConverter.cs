using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using LightBulb.Models;

namespace LightBulb.Converters
{
    /// <summary>
    /// Converts Hotkey to string
    /// </summary>
    [ValueConversion(typeof(Hotkey), typeof(string))]
    public class HotkeyToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "< not set >";

            var hotkey = (Hotkey) value;
            var key = (Key) hotkey.Key;
            var modifiers = (ModifierKeys) hotkey.Modifiers;

            var str = new StringBuilder();
            if (modifiers.HasFlag(ModifierKeys.Control))
                str.Append("Ctrl + ");
            if (modifiers.HasFlag(ModifierKeys.Shift))
                str.Append("Shift + ");
            if (modifiers.HasFlag(ModifierKeys.Alt))
                str.Append("Alt + ");
            if (modifiers.HasFlag(ModifierKeys.Windows))
                str.Append("Win + ");
            str.Append(key);
            return str.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}