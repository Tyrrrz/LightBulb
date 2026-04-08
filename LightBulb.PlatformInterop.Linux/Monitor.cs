using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CliWrap;

namespace LightBulb.PlatformInterop;

public partial class Monitor(string outputName) : IDisposable
{
    public Rect? TryGetBounds() => null;

    public string? TryGetDeviceName() => outputName;

    public DeviceContext? TryCreateDeviceContext() => DeviceContext.TryCreate(outputName);

    public void Dispose() { }
}

public partial class Monitor
{
    // Matches lines like: " 0: +*HDMI-1 1920/527x1080/296+0+0  HDMI-1"
    private static readonly Regex MonitorLineRegex = new(
        @"^\s*\d+:\s+\+[*+]?(\S+)",
        RegexOptions.Compiled
    );

    public static async Task<IReadOnlyList<Monitor>> GetAllAsync()
    {
        var monitors = new List<Monitor>();

        try
        {
            var stdout = new StringBuilder();

            await Cli.Wrap("xrandr")
                .WithArguments(["--listmonitors"])
                .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdout))
                .ExecuteAsync();

            foreach (var line in stdout.ToString().Split('\n'))
            {
                var match = MonitorLineRegex.Match(line);
                if (match.Success)
                    monitors.Add(new Monitor(match.Groups[1].Value));
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to enumerate monitors via xrandr: {ex.Message}");
        }

        return monitors;
    }
}
