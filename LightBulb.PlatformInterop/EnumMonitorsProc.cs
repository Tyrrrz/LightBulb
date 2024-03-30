namespace LightBulb.PlatformInterop;

internal delegate bool EnumMonitorsProc(
    nint hMonitor,
    nint hdcMonitor,
    Rect lprcMonitor,
    nint dwData
);
