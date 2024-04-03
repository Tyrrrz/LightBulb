using System.Runtime.InteropServices;

namespace LightBulb.PlatformInterop.Internal;

[StructLayout(LayoutKind.Sequential)]
internal readonly record struct MonitorInfoEx
{
    public MonitorInfoEx() => Size = Marshal.SizeOf(this);

    public int Size { get; }
    public Rect Monitor { get; init; }
    public Rect WorkArea { get; init; }
    public uint Flags { get; init; }

    [field: MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string? DeviceName { get; init; }
}
