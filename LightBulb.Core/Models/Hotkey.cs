using System;

namespace LightBulb.Models
{
    /// <summary>
    /// Combination of a key and modifiers
    /// </summary>
    public class Hotkey : IEquatable<Hotkey>
    {
        /// <summary>
        /// Key
        /// </summary>
        public int Key { get; }

        /// <summary>
        /// Modifiers
        /// </summary>
        public int Modifiers { get; }

        public Hotkey(int key, int modifiers)
        {
            Key = key;
            Modifiers = modifiers;
        }

        public override bool Equals(object obj)
        {
            return Equals(this, obj as Hotkey);
        }

        public bool Equals(Hotkey other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Key*397) ^ Modifiers;
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