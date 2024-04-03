using System.Collections.Generic;
using System.Diagnostics;
using LightBulb.PlatformInterop.Internal;

namespace LightBulb.PlatformInterop;

public partial class Monitor(nint handle) : NativeResource(handle)
{
    private MonitorInfoEx? TryGetMonitorInfo()
    {
        var monitorInfo = new MonitorInfoEx();

        if (!NativeMethods.GetMonitorInfo(Handle, ref monitorInfo))
        {
            Debug.WriteLine($"Failed to retrieve info for monitor #{Handle}.");
            return null;
        }

        return monitorInfo;
    }

    public Rect? TryGetBounds() => TryGetMonitorInfo()?.Monitor;

    public string? TryGetDeviceName() => TryGetMonitorInfo()?.DeviceName;

    public DeviceContext? TryCreateDeviceContext()
    {
        var name = TryGetDeviceName();
        if (string.IsNullOrWhiteSpace(name))
            return null;

        return DeviceContext.TryCreate(name);
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
