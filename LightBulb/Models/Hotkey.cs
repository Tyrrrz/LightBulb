using System.Text;
using System.Windows.Input;

namespace LightBulb.Models
{
    public class Hotkey
    {
        public Key Key { get; }
        public ModifierKeys Modifiers { get; }

        public Hotkey(Key key, ModifierKeys modifiers = ModifierKeys.None)
        {
            Key = key;
            Modifiers = modifiers;
        }

        public override string ToString()
        {
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
            return Equals(this, obj as Hotkey);
        }

        protected bool Equals(Hotkey other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) Key*397) ^ (int) Modifiers;
            }
        }

        public static bool operator ==(Hotkey a, Hotkey b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (ReferenceEquals(a, null)) return false;
            if (ReferenceEquals(b, null)) return false;
            return a.Key == b.Key && a.Modifiers == b.Modifiers;
        }

        public static bool operator !=(Hotkey a, Hotkey b) => !(a == b);
    }
}