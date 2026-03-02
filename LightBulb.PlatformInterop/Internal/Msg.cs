using System.Runtime.InteropServices;

namespace LightBulb.PlatformInterop.Internal;

[StructLayout(LayoutKind.Sequential)]
internal struct Msg
{
    public nint HWnd;
    public uint Message;
    public nint WParam;
    public nint LParam;
    public uint Time;
    public int PtX;
    public int PtY;
    public uint Private;
}
