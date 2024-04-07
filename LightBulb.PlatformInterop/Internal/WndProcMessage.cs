using System.Runtime.InteropServices;

namespace LightBulb.PlatformInterop.Internal;

internal readonly record struct WndProcMessage(uint Id, nint WParam, nint LParam)
{
    public T? TryGetLParam<T>()
        where T : struct =>
        // If LParam is zero, marshaling will fail with null reference exception
        LParam != 0
            ? Marshal.PtrToStructure<T>(LParam)
            : default(T?);
}
