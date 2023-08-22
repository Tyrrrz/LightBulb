using System;

namespace LightBulb.Core;

public readonly partial record struct ColorConfiguration(double Temperature, double Brightness)
{
    public ColorConfiguration WithOffset(double temperatureOffset, double brightnessOffset) =>
        new(Temperature + temperatureOffset, Brightness + brightnessOffset);

    public ColorConfiguration Clamp(
        double minimumTemperature,
        double maximumTemperature,
        double minimumBrightness,
        double maximumBrightness
    ) =>
        new(
            Math.Clamp(Temperature, minimumTemperature, maximumTemperature),
            Math.Clamp(Brightness, minimumBrightness, maximumBrightness)
        );
}

public partial record struct ColorConfiguration
{
    public static ColorConfiguration Default { get; } = new(6600, 1);
}
