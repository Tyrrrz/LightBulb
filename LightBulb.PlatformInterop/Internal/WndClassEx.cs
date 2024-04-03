using System.Runtime.InteropServices;

namespace LightBulb.PlatformInterop.Internal;

[StructLayout(LayoutKind.Sequential)]
internal readonly record struct WndClassEx
{
    public WndClassEx()
    {
        Size = (uint)Marshal.SizeOf(this);
        Instance = Marshal.GetHINSTANCE(typeof(WndClassEx).Module);
    }

    public uint Size { get; }
    public uint Style { get; init; }
    public required WndProc WndProc { get; init; }
    public int ClassExtra { get; init; }
    public int WindowExtra { get; init; }
    public nint Instance { get; }
    public nint Icon { get; init; }
    public nint Cursor { get; init; }
    public nint Background { get; init; }
    public string? MenuName { get; init; }
    public required string ClassName { get; init; }
    public nint IconSm { get; init; }
}
