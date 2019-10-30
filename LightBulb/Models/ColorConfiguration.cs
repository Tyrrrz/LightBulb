using System;

namespace LightBulb.Models
{
    public partial struct ColorConfiguration : IEquatable<ColorConfiguration>
    {
        public double Temperature { get; }

        public double Brightness { get; }

        public ColorConfiguration(double temperature, double brightness)
        {
            Temperature = temperature;
            Brightness = brightness;
        }

        public bool Equals(ColorConfiguration other) => Temperature.Equals(other.Temperature) && Brightness.Equals(other.Brightness);

        public override bool Equals(object obj) => obj is ColorConfiguration other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Temperature, Brightness);

        public override string ToString() => $"{Temperature:F0} K / {Brightness:P0}";
    }

    public partial struct ColorConfiguration
    {
        public static bool operator ==(ColorConfiguration a, ColorConfiguration b) => a.Equals(b);

        public static bool operator !=(ColorConfiguration a, ColorConfiguration b) => !(a == b);
    }

    public partial struct ColorConfiguration
    {
        public static ColorConfiguration Default { get; } = new ColorConfiguration(6600, 1);
    }
}