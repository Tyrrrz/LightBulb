using System.Runtime.InteropServices;

namespace LightBulb.PlatformInterop.Internal;

[StructLayout(LayoutKind.Sequential)]
internal struct MonitorInfoEx
{
    public int Size;
    public Rect Monitor;
    public Rect WorkArea;
    public uint Flags;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string DeviceName;
}
