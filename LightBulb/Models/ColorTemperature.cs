using System;
using LightBulb.Internal;

namespace LightBulb.Models
{
    public partial struct ColorTemperature : IEquatable<ColorTemperature>, IComparable<ColorTemperature>
    {
        public double Value { get; }

        public ColorTemperature(double value)
        {
            Value = value.GuardNotNegative(nameof(value));
        }

        public double GetRedMultiplier()
        {
            // Original code credit: http://www.tannerhelland.com/4435/convert-temperature-rgb-algorithm-code/

            if (Value > 6600)
                return Math.Pow(Value / 100 - 60, -0.1332047592) * 329.698727446 / 255;

            return 1;
        }

        public double GetGreenMultiplier()
        {
            // Original code credit: http://www.tannerhelland.com/4435/convert-temperature-rgb-algorithm-code/

            if (Value > 6600)
                return Math.Pow(Value / 100 - 60, -0.0755148492) * 288.1221695283 / 255;

            return (Math.Log(Value / 100) * 99.4708025861 - 161.1195681661) / 255;
        }

        public double GetBlueMultiplier()
        {
            // Original code credit: http://www.tannerhelland.com/4435/convert-temperature-rgb-algorithm-code/

            if (Value >= 6600)
                return 1;

            if (Value <= 1900)
                return 0;

            return (Math.Log(Value / 100 - 10) * 138.5177312231 - 305.0447927307) / 255;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            return obj is ColorTemperature other && Equals(other);
        }

        public override int GetHashCode() => Value.GetHashCode();

        public bool Equals(ColorTemperature other) => Value.Equals(other.Value);

        public int CompareTo(ColorTemperature other) => Value.CompareTo(other.Value);

        public override string ToString() => $"{Value:F0}K";
    }

    public partial struct ColorTemperature
    {
        public static bool operator ==(ColorTemperature a, ColorTemperature b) => a.Equals(b);

        public static bool operator !=(ColorTemperature a, ColorTemperature b) => !(a == b);

        public static bool operator >(ColorTemperature a, ColorTemperature b) => a.CompareTo(b) > 0;

        public static bool operator <(ColorTemperature a, ColorTemperature b) => a.CompareTo(b) < 0;
    }
}