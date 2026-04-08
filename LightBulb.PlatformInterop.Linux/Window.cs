using System;
using System.Collections.Generic;

namespace LightBulb.PlatformInterop;

public partial class Window : IDisposable
{
    private Window() { }

    public Process? TryGetProcess() => null;

    public Monitor? TryGetMonitor() => null;

    public Rect? TryGetRect() => null;

    public Rect? TryGetClientRect() => null;

    public string? TryGetClassName() => null;

    public bool IsSystemWindow() => false;

    public bool IsVisible() => false;

    public bool IsFullScreen() => false;

    public void Dispose() { }
}

public partial class Window
{
    public static Window? TryGetForeground() => null;

    public static IReadOnlyList<Window> GetAll() => [];
}
