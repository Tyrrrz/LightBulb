using System;

namespace LightBulb.Models
{
    public readonly partial struct ColorTemperature : IEquatable<ColorTemperature>, IComparable<ColorTemperature>
    {
        public double Value { get; }

        public ColorTemperature(double value)
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            return obj is ColorTemperature other && Equals(other);
        }

        public bool Equals(ColorTemperature other) => Value.Equals(other.Value);

        public int CompareTo(ColorTemperature other) => Value.CompareTo(other.Value);

        public override int GetHashCode() => HashCode.Combine(Value);

        public override string ToString() => $"{Value:F0}K";
    }

    public partial struct ColorTemperature
    {
        public static bool operator ==(ColorTemperature a, ColorTemperature b) => a.Equals(b);

        public static bool operator !=(ColorTemperature a, ColorTemperature b) => !(a == b);

        public static bool operator >(ColorTemperature a, ColorTemperature b) => a.CompareTo(b) > 0;

        public static bool operator <(ColorTemperature a, ColorTemperature b) => a.CompareTo(b) < 0;
    }

    public partial struct ColorTemperature
    {
        public static ColorTemperature Default { get; } = new ColorTemperature(6600);
    }
}