using System.Runtime.InteropServices;

namespace LightBulb.PlatformInterop.Internal;

internal static class NativeModule
{
    public static nint Handle { get; } = Marshal.GetHINSTANCE(typeof(NativeModule).Module);
}
