using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CliWrap;

namespace LightBulb.PlatformInterop;

public partial class DeviceContext(string outputName) : IDisposable
{
    public async ValueTask SetGammaAsync(
        double redMultiplier,
        double greenMultiplier,
        double blueMultiplier
    )
    {
        // Use xrandr to apply per-channel gamma on X11/XWayland
        var gamma = FormattableString.Invariant(
            $"{redMultiplier:F4}:{greenMultiplier:F4}:{blueMultiplier:F4}"
        );

        try
        {
            await Cli.Wrap("xrandr")
                .WithArguments(["--output", outputName, "--gamma", gamma])
                .ExecuteAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to set gamma for '{outputName}': {ex.Message}");
        }
    }

    public ValueTask ResetGammaAsync() => SetGammaAsync(1, 1, 1);

    public void Dispose() { }
}

public partial class DeviceContext
{
    public static DeviceContext? TryCreate(string outputName) =>
        !string.IsNullOrWhiteSpace(outputName) ? new DeviceContext(outputName) : null;
}
