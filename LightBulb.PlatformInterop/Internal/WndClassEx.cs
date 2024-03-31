using System.Runtime.InteropServices;

namespace LightBulb.PlatformInterop.Internal;

[StructLayout(LayoutKind.Sequential)]
internal record struct WndClassEx
{
    public uint Size { get; init; }
    public uint Style { get; init; }
    public WndProc WndProc { get; init; }
    public int ClassExtra { get; init; }
    public int WindowExtra { get; init; }
    public nint Instance { get; init; }
    public nint Icon { get; init; }
    public nint Cursor { get; init; }
    public nint Background { get; init; }
    public string? MenuName { get; init; }
    public string ClassName { get; init; }
    public nint IconSm { get; init; }
}