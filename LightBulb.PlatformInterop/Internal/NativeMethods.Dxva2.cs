using System;
using System.Runtime.InteropServices;

namespace LightBulb.PlatformInterop.Internal;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public struct PhysicalMonitor
{
    public nint Handle;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string Description;
}

internal static partial class NativeMethods
{
    [DllImport("dxva2.dll", SetLastError = true)]
    public static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(
        nint hMonitor,
        out uint pdwNumberOfPhysicalMonitors
    );

    [DllImport("dxva2.dll", SetLastError = true)]
    public static extern bool GetPhysicalMonitorsFromHMONITOR(
        nint hMonitor,
        uint dwPhysicalMonitorArraySize,
        [Out] PhysicalMonitor[] pPhysicalMonitorArray
    );

    [DllImport("dxva2.dll", SetLastError = true)]
    public static extern bool SetMonitorBrightness(nint hMonitor, uint dwNewBrightness);

    [DllImport("dxva2.dll", SetLastError = true)]
    public static extern bool DestroyPhysicalMonitors(
        uint dwPhysicalMonitorArraySize,
        [In] PhysicalMonitor[] pPhysicalMonitorArray
    );
}
