using System;
using Tyrrrz.Extensions;

namespace LightBulb.Domain
{
    public readonly partial struct ColorConfiguration
    {
        public double Temperature { get; }

        public double Brightness { get; }

        public ColorConfiguration(double temperature, double brightness)
        {
            Temperature = temperature.Clamp(MinTemperature, MaxTemperature);
            Brightness = brightness.Clamp(MinBrightness, MaxBrightness);
        }

        public ColorConfiguration WithOffset(double temperatureOffset, double brightnessOffset) => new ColorConfiguration(
            Temperature + temperatureOffset,
            Brightness + brightnessOffset
        );

        public override string ToString() => $"{Temperature:F0} K, {Brightness:P0}";
    }

    public partial struct ColorConfiguration
    {
        public static double MinTemperature { get; } = 500;

        public static double MaxTemperature { get; } = 20_000;

        public static double MinBrightness { get; } = 0.1;

        public static double MaxBrightness { get; } = 1;

        public static ColorConfiguration Default { get; } = new ColorConfiguration(6600, 1);
    }

    public partial struct ColorConfiguration : IEquatable<ColorConfiguration>
    {
        public bool Equals(ColorConfiguration other) =>
            Temperature.Equals(other.Temperature) && Brightness.Equals(other.Brightness);

        public override bool Equals(object? obj) =>
            obj is ColorConfiguration other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Temperature, Brightness);

        public static bool operator ==(ColorConfiguration a, ColorConfiguration b) => a.Equals(b);

        public static bool operator !=(ColorConfiguration a, ColorConfiguration b) => !(a == b);
    }
}