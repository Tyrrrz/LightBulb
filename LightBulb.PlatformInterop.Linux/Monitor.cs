using System;
using System.Collections.Generic;

namespace LightBulb.PlatformInterop;

public partial class Monitor : IDisposable
{
    private Monitor() { }

    public Rect? TryGetBounds() => null;

    public string? TryGetDeviceName() => null;

    public DeviceContext? TryCreateDeviceContext() => null;

    public void Dispose() { }
}

public partial class Monitor
{
    public static IReadOnlyList<Monitor> GetAll() => [];
}
