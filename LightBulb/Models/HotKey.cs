using System;
using System.Text;
using System.Windows.Input;

namespace LightBulb.Models
{
    public readonly partial struct HotKey
    {
        public Key Key { get; }

        public ModifierKeys Modifiers { get; }

        public HotKey(Key key, ModifierKeys modifiers = ModifierKeys.None)
        {
            Key = key;
            Modifiers = modifiers;
        }

        public override string ToString()
        {
            if (Key == Key.None && Modifiers == ModifierKeys.None)
                return "< None >";

            var buffer = new StringBuilder();

            if (Modifiers.HasFlag(ModifierKeys.Control))
                buffer.Append("Ctrl + ");
            if (Modifiers.HasFlag(ModifierKeys.Shift))
                buffer.Append("Shift + ");
            if (Modifiers.HasFlag(ModifierKeys.Alt))
                buffer.Append("Alt + ");
            if (Modifiers.HasFlag(ModifierKeys.Windows))
                buffer.Append("Win + ");

            buffer.Append(Key);

            return buffer.ToString();
        }
    }

    public partial struct HotKey
    {
        public static HotKey None { get; } = new();
    }

    public partial struct HotKey : IEquatable<HotKey>
    {
        public bool Equals(HotKey other) => Key == other.Key && Modifiers == other.Modifiers;

        public override bool Equals(object? obj) => obj is HotKey other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Key, Modifiers);

        public static bool operator ==(HotKey a, HotKey b) => a.Equals(b);

        public static bool operator !=(HotKey a, HotKey b) => !(a == b);
    }
}