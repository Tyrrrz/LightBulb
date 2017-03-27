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
            if (ReferenceEquals(this, obj)) return true;
            if (ReferenceEquals(obj, null)) return false;
            return Equals(obj as Hotkey);
        }

        public bool Equals(Hotkey other)
        {
            if (ReferenceEquals(other, null)) return false;
            return Key == other.Key && Modifiers == other.Modifiers;
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
            return a.Equals(b);
        }

        public static bool operator !=(Hotkey a, Hotkey b) => !(a == b);
    }
}