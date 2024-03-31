using System.Collections.Generic;
using System.Diagnostics;
using LightBulb.PlatformInterop.Internal;

namespace LightBulb.PlatformInterop;

public partial class Monitor(nint handle) : NativeResource(handle)
{
    private MonitorInfoEx? TryGetMonitorInfo()
    {
        if (!NativeMethods.GetMonitorInfo(Handle, out var monitorInfo))
        {
            Debug.WriteLine($"Failed to retrieve monitor info for monitor #{Handle}.");
            return null;
        }

        return monitorInfo;
    }
    
    public string? TryGetName() => TryGetMonitorInfo()?.DeviceName;

    public Rect? TryGetBounds() => TryGetMonitorInfo()?.Monitor;

    public DeviceContext? TryCreateDeviceContext()
    {
        var name = TryGetName();
        if (string.IsNullOrWhiteSpace(name))
            return null;
        
        return DeviceContext.TryCreateForDevice(name);
    }

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
