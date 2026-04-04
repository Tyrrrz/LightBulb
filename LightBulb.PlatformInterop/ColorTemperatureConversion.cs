using System;

namespace LightBulb.PlatformInterop;

/// <summary>
/// Converts a color temperature (in Kelvin) to linear RGB multipliers suitable
/// for a hardware gamma ramp. Algorithm from
/// http://tannerhelland.com/4435/convert-temperature-rgb-algorithm-code
/// </summary>
public static class ColorTemperatureConversion
{
    public static double GetRedMultiplier(double temperature)
    {
        if (temperature > 6600)
        {
            return Math.Clamp(
                Math.Pow(temperature / 100 - 60, -0.1332047592) * 329.698727446 / 255,
                0,
                1
            );
        }

        return 1;
    }

    public static double GetGreenMultiplier(double temperature)
    {
        if (temperature > 6600)
        {
            return Math.Clamp(
                Math.Pow(temperature / 100 - 60, -0.0755148492) * 288.1221695283 / 255,
                0,
                1
            );
        }

        return Math.Clamp(
            (Math.Log(temperature / 100) * 99.4708025861 - 161.1195681661) / 255,
            0,
            1
        );
    }

    public static double GetBlueMultiplier(double temperature)
    {
        if (temperature >= 6600)
            return 1;

        if (temperature <= 1900)
            return 0;

        return Math.Clamp(
            (Math.Log(temperature / 100 - 10) * 138.5177312231 - 305.0447927307) / 255,
            0,
            1
        );
    }
}
