using System.Text;
using System.Windows.Input;

namespace LightBulb.Models
{
    public struct Hotkey
    {
        public static readonly Hotkey Unset = new Hotkey(Key.None);

        public Key Key { get; set; }

        public ModifierKeys Modifiers { get; set; }

        public Hotkey(Key key, ModifierKeys modifiers = ModifierKeys.None)
        {
            Key = key;
            Modifiers = modifiers;
        }

        public override string ToString()
        {
            // Unset
            if (Key == Key.None && Modifiers == ModifierKeys.None) return "< not set >";

            // Set
            var str = new StringBuilder();
            if (Modifiers.HasFlag(ModifierKeys.Control))
                str.Append("Ctrl + ");
            if (Modifiers.HasFlag(ModifierKeys.Shift))
                str.Append("Shift + ");
            if (Modifiers.HasFlag(ModifierKeys.Alt))
                str.Append("Alt + ");
            if (Modifiers.HasFlag(ModifierKeys.Windows))
                str.Append("Win + ");
            str.Append(Key);
            return str.ToString();
        }
    }
}