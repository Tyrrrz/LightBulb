using System.Text;
using System.Windows.Input;
using Newtonsoft.Json;

namespace LightBulb.Models
{
    public struct Hotkey
    {
        public static readonly Hotkey Unset = new Hotkey(Key.None);

        public Key Key { get; }

        public ModifierKeys Modifiers { get; }

        [JsonConstructor] // for Tyrrrz.Settings' Json.Net deserializer
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

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public bool Equals(Hotkey other)
        {
            return Key == other.Key && Modifiers == other.Modifiers;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) Key*397) ^ (int) Modifiers;
            }
        }

        public static bool operator ==(Hotkey a, Hotkey b) => a.Equals(b);

        public static bool operator !=(Hotkey a, Hotkey b) => !(a == b);
    }
}