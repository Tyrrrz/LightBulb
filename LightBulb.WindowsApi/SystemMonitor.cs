using System.Collections.Generic;
using System.Diagnostics;
using LightBulb.WindowsApi.Native;

namespace LightBulb.WindowsApi;

public partial class SystemMonitor(nint handle) : NativeResource(handle)
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

    internal Rect? TryGetBounds()
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

public partial class SystemMonitor
{
    public static IReadOnlyList<SystemMonitor> GetAll()
    {
        var monitors = new List<SystemMonitor>();

        if (
            !NativeMethods.EnumDisplayMonitors(
                0,
                0,
                (hMonitor, _, _, _) =>
                {
                    monitors.Add(new SystemMonitor(hMonitor));
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
