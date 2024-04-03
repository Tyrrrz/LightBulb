using System.Runtime.InteropServices;

namespace LightBulb.PlatformInterop.Internal;

[StructLayout(LayoutKind.Sequential)]
internal record struct WndClassEx
{
    public uint Size;
    public uint Style;
    public WndProc WndProc;
    public int ClassExtra;
    public int WindowExtra;
    public nint Instance;
    public nint Icon;
    public nint Cursor;
    public nint Background;
    public string? MenuName;
    public string ClassName;
    public nint IconSm;
}
