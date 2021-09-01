using System;
using Tyrrrz.Extensions;

namespace LightBulb.Core
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

        public ColorConfiguration WithOffset(double temperatureOffset, double brightnessOffset) => new(
            Temperature + temperatureOffset,
            Brightness + brightnessOffset
        );

        public override string ToString() => $"{Temperature:F0} K, {Brightness:P0}";
    }

    public partial struct ColorConfiguration
    {
        public static double MinTemperature => 500;

        public static double MaxTemperature => 20_000;

        public static double MinBrightness => 0.1;

        public static double MaxBrightness => 1;

        public static ColorConfiguration Default { get; } = new(6600, 1);
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