using System;
using System.Runtime.InteropServices;

namespace LightBulb.PlatformInterop.Internal;

internal readonly record struct WndProcMessage(uint Id, nint WParam, nint LParam)
{
    public T DeserializeLParam<T>() =>
        (T?)Marshal.PtrToStructure(LParam, typeof(T))
        ?? throw new InvalidOperationException(
            $"Failed to deserialize WndProc message's lParam to {typeof(T)}."
        );
}
