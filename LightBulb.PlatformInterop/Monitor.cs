using System.Collections.Generic;
using System.Diagnostics;

namespace LightBulb.PlatformInterop;

public partial class Monitor(nint handle) : NativeResource(handle)
{
    public string? TryGetName()
    {
        if (!NativeMethods.GetMonitorInfo(Handle, out var monitorInfo))
        {
            Debug.WriteLine($"Failed to retrieve monitor info for monitor #{Handle}.");
            return null;
        }

        return monitorInfo.DeviceName;
    }

    public Rect? TryGetBounds()
    {
        if (!NativeMethods.GetMonitorInfo(Handle, out var monitorInfo))
        {
            Debug.WriteLine($"Failed to retrieve monitor info for monitor #{Handle}.");
            return null;
        }

        return monitorInfo.Monitor;
    }

    public DeviceContext? TryGetDeviceContext() =>
        TryGetName() is { } name ? DeviceContext.TryGetByName(name) : null;

    protected override void Dispose(bool disposing) { }
}

public partial class Monitor
{
    public static IReadOnlyList<Monitor> GetAll()
    {
        var monitors = new List<Monitor>();

        if (
            !NativeMethods.EnumDisplayMonitors(
                0,
                0,
                (hMonitor, _, _, _) =>
                {
                    monitors.Add(new Monitor(hMonitor));
                    return true;
                },
                0
            )
        )
        {
            Debug.WriteLine("Failed to enumerate display monitors.");
        }

        return monitors;
    }
}
