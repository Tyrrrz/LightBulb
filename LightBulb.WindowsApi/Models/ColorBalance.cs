using System;

namespace LightBulb.WindowsApi.Models
{
    public readonly partial struct ColorBalance : IEquatable<ColorBalance>
    {
        public double Red { get; }

        public double Green { get; }

        public double Blue { get; }

        public ColorBalance(double red, double green, double blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public bool Equals(ColorBalance other) => Red.Equals(other.Red) && Green.Equals(other.Green) && Blue.Equals(other.Blue);

        public override bool Equals(object? obj) => obj is ColorBalance other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Red, Green, Blue);

        public override string ToString() => $"R:{Red} G:{Green} B:{Blue}";
    }

    public partial struct ColorBalance
    {
        public static bool operator ==(ColorBalance a, ColorBalance b) => a.Equals(b);

        public static bool operator !=(ColorBalance a, ColorBalance b) => !(a == b);
    }

    public partial struct ColorBalance
    {
        public static ColorBalance Default { get; } = new ColorBalance(1, 1, 1);
    }
}