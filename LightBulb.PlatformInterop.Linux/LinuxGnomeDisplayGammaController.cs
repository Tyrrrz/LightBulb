using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CliWrap;
using LightBulb.Core;

namespace LightBulb.PlatformInterop;

/// <summary>
/// Controls color temperature on Linux via GNOME Night Light (gsettings).
/// Delegates native display calls to the GNOME compositor, keeping system settings in sync.
/// Brightness is not supported — GNOME Night Light is temperature-only.
/// </summary>
public class LinuxGnomeDisplayGammaController : IDisplayColorController
{
    public string Id => "linux-gnome";
    public bool IsBrightnessSupported => false;

    public static bool IsAvailable =>
        Environment
            .GetEnvironmentVariable("XDG_CURRENT_DESKTOP")
            ?.Contains("GNOME", StringComparison.OrdinalIgnoreCase) == true;

    public async ValueTask SetGammaAsync(ColorConfiguration configuration)
    {
        try
        {
            // Enable night light so the temperature setting takes effect
            await Cli.Wrap("gsettings")
                .WithArguments([
                    "set",
                    "org.gnome.settings-daemon.plugins.color",
                    "night-light-enabled",
                    "true",
                ])
                .ExecuteAsync();

            // Set the target temperature (gsettings accepts integers in Kelvin)
            await Cli.Wrap("gsettings")
                .WithArguments([
                    "set",
                    "org.gnome.settings-daemon.plugins.color",
                    "night-light-temperature",
                    ((int)Math.Round(configuration.Temperature)).ToString(),
                ])
                .ExecuteAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[{Id}] Failed to set gamma: {ex.Message}");
        }
    }

    public async ValueTask ResetGammaAsync()
    {
        try
        {
            await Cli.Wrap("gsettings")
                .WithArguments([
                    "set",
                    "org.gnome.settings-daemon.plugins.color",
                    "night-light-enabled",
                    "false",
                ])
                .ExecuteAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[{Id}] Failed to reset gamma: {ex.Message}");
        }
    }

    public void Invalidate() { }

    public void Dispose() { }
}
