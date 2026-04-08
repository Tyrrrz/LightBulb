using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CliWrap;
using LightBulb.Core;

namespace LightBulb.PlatformInterop;

/// <summary>
/// Controls color temperature on Linux via KDE Plasma Night Color (kwriteconfig + qdbus).
/// Delegates native display calls to the KWin compositor, keeping system settings in sync.
/// Brightness is not supported — KDE Night Color is temperature-only.
/// </summary>
public class LinuxPlasmaDisplayGammaController : IDisplayColorController
{
    // kwriteconfig executable name: Plasma 5 uses kwriteconfig5, Plasma 6 uses kwriteconfig6.
    // Try kwriteconfig6 first (newer), fall back to kwriteconfig5.
    private static readonly string[] KWriteConfigCandidates = ["kwriteconfig6", "kwriteconfig5"];

    // qdbus executable name: Plasma 6 uses qdbus6, Plasma 5 uses qdbus.
    private static readonly string[] DbusCandidates = ["qdbus6", "qdbus"];

    public string Id => "linux-plasma";
    public bool IsBrightnessSupported => false;

    public static bool IsAvailable =>
        Environment
            .GetEnvironmentVariable("XDG_CURRENT_DESKTOP")
            ?.Contains("KDE", StringComparison.OrdinalIgnoreCase) == true;

    private static async Task<bool> TryRunKWriteConfigAsync(params string[] args)
    {
        foreach (var exe in KWriteConfigCandidates)
        {
            try
            {
                await Cli.Wrap(exe).WithArguments(args).ExecuteAsync();
                return true;
            }
            catch
            {
                // Try next candidate
            }
        }

        return false;
    }

    private static async Task TryReconfigureKWinAsync()
    {
        // Signal KWin to pick up changed night-color settings.
        // Try qdbus6 (Plasma 6), then qdbus (Plasma 5), then dbus-send as a last resort.
        foreach (var dbus in DbusCandidates)
        {
            try
            {
                await Cli.Wrap(dbus)
                    .WithArguments([
                        "org.kde.KWin",
                        "/ColorCorrect",
                        "org.kde.kwin.ColorCorrect.reconfigure",
                    ])
                    .ExecuteAsync();
                return;
            }
            catch
            {
                // Try next
            }
        }

        // Fallback: dbus-send
        try
        {
            await Cli.Wrap("dbus-send")
                .WithArguments([
                    "--session",
                    "--dest=org.kde.KWin",
                    "--type=method_call",
                    "/ColorCorrect",
                    "org.kde.kwin.ColorCorrect.reconfigure",
                ])
                .ExecuteAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[KDE Plasma Night Color] dbus-send reconfigure failed: {ex.Message}");
        }
    }

    public async ValueTask SetGammaAsync(ColorConfiguration configuration)
    {
        var temperature = ((int)Math.Round(configuration.Temperature)).ToString();

        try
        {
            // Mode 3 = "Always on" (constant temperature, ignores time/location schedule)
            if (
                await TryRunKWriteConfigAsync(
                    "--file",
                    "kwinrc",
                    "--group",
                    "NightColor",
                    "--key",
                    "Mode",
                    "3"
                )
                && await TryRunKWriteConfigAsync(
                    "--file",
                    "kwinrc",
                    "--group",
                    "NightColor",
                    "--key",
                    "NightTemperature",
                    temperature
                )
            )
            {
                await TryReconfigureKWinAsync();
            }
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
            // Restore Mode to "Automatic" (0) which follows the sunset/sunrise schedule,
            // effectively turning off the forced temperature.
            if (
                await TryRunKWriteConfigAsync(
                    "--file",
                    "kwinrc",
                    "--group",
                    "NightColor",
                    "--key",
                    "Mode",
                    "0"
                )
            )
                await TryReconfigureKWinAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[{Id}] Failed to reset gamma: {ex.Message}");
        }
    }

    public void Invalidate() { }

    public void Dispose() { }
}
