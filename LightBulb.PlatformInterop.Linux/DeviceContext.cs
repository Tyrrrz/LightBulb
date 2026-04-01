using System;
using System.Diagnostics;
using SystemProcess = System.Diagnostics.Process;

namespace LightBulb.PlatformInterop;

public partial class DeviceContext(string outputName) : IDisposable
{
    public void SetGamma(double redMultiplier, double greenMultiplier, double blueMultiplier)
    {
        // Use xrandr to apply per-channel gamma on X11/XWayland
        var gamma = FormattableString.Invariant(
            $"{redMultiplier:F4}:{greenMultiplier:F4}:{blueMultiplier:F4}"
        );

        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "xrandr",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            startInfo.ArgumentList.Add("--output");
            startInfo.ArgumentList.Add(outputName);
            startInfo.ArgumentList.Add("--gamma");
            startInfo.ArgumentList.Add(gamma);

            using var process = SystemProcess.Start(startInfo);
            process?.WaitForExit();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to set gamma for '{outputName}': {ex.Message}");
        }
    }

    public void ResetGamma() => SetGamma(1, 1, 1);

    public void Dispose() { }
}

public partial class DeviceContext
{
    public static DeviceContext? TryCreate(string outputName) =>
        !string.IsNullOrWhiteSpace(outputName) ? new DeviceContext(outputName) : null;
}
