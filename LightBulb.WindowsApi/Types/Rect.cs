using System.Runtime.InteropServices;

namespace LightBulb.WindowsApi.Types;

[StructLayout(LayoutKind.Sequential)]
internal readonly record struct Rect(int Left, int Top, int Right, int Bottom)
{
    public static Rect Empty { get; } = new();
}
