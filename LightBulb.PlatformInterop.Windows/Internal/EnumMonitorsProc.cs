namespace LightBulb.PlatformInterop.Internal;

internal delegate bool EnumMonitorsProc(
    nint hMonitor,
    nint hdcMonitor,
    Rect lprcMonitor,
    nint dwData
);
