using System.Runtime.InteropServices;

namespace LightBulb.WindowsApi.Native;

[StructLayout(LayoutKind.Sequential)]
internal readonly record struct MonitorInfoEx(
    int Size,
    Rect Monitor,
    Rect Work,
    uint Flags,
    string DeviceName
);
