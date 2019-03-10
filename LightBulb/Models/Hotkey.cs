﻿using System;
using System.Text;
using System.Windows.Input;

namespace LightBulb.Models
{
    public partial struct HotKey : IEquatable<HotKey>
    {
        public Key Key { get; }

        public ModifierKeys Modifiers { get; }

        public HotKey(Key key, ModifierKeys modifiers = ModifierKeys.None)
        {
            Key = key;
            Modifiers = modifiers;
        }

        public bool Equals(HotKey other) => Key == other.Key && Modifiers == other.Modifiers;

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            return obj is HotKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) Key * 397) ^ (int) Modifiers;
            }
        }

        public override string ToString()
        {
            var buffer = new StringBuilder();

            // Modifiers
            if (Modifiers.HasFlag(ModifierKeys.Control))
                buffer.Append("Ctrl + ");
            if (Modifiers.HasFlag(ModifierKeys.Shift))
                buffer.Append("Shift + ");
            if (Modifiers.HasFlag(ModifierKeys.Alt))
                buffer.Append("Alt + ");
            if (Modifiers.HasFlag(ModifierKeys.Windows))
                buffer.Append("Win + ");

            // Key
            buffer.Append(Key);

            return buffer.ToString();
        }
    }

    public partial struct HotKey
    {
        public static bool operator ==(HotKey a, HotKey b) => a.Equals(b);

        public static bool operator !=(HotKey a, HotKey b) => !(a == b);
    }
}