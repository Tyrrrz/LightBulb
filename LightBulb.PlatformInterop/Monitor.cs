using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using LightBulb.PlatformInterop.Internal;

namespace LightBulb.PlatformInterop;

public partial class Monitor(nint handle) : NativeResource(handle)
{
    private PhysicalMonitor[]? TryGetPhysicalMonitors()
    {
        if (!NativeMethods.GetNumberOfPhysicalMonitorsFromHMONITOR(Handle, out var count))
        {
            Debug.WriteLine(
                $"Failed to get physical monitor count for monitor #{Handle}. "
                    + $"Error {Marshal.GetLastWin32Error()}."
            );

            return null;
        }

        if (count == 0)
            return [];

        var monitors = new PhysicalMonitor[count];
        if (!NativeMethods.GetPhysicalMonitorsFromHMONITOR(Handle, count, monitors))
        {
            Debug.WriteLine(
                $"Failed to get physical monitors for monitor #{Handle}. "
                    + $"Error {Marshal.GetLastWin32Error()}."
            );

            return null;
        }

        return monitors;
    }

    private MonitorInfoEx? TryGetMonitorInfo()
    {
        var monitorInfo = new MonitorInfoEx();

        if (!NativeMethods.GetMonitorInfo(Handle, ref monitorInfo))
        {
            Debug.WriteLine(
                $"Failed to retrieve info for monitor #{Handle}. "
                    + $"Error {Marshal.GetLastWin32Error()}."
            );

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

        var physicalMonitors = TryGetPhysicalMonitors();
        if (physicalMonitors is null)
            return null;

        return DeviceContext.TryCreate(name, physicalMonitors);
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
            Debug.WriteLine(
                "Failed to enumerate display monitors. " + $"Error {Marshal.GetLastWin32Error()}."
            );
        }

        return monitors;
    }
}
