namespace LightBulb.WindowsApi.Native;

internal delegate bool EnumMonitorsProc(
    nint hMonitor,
    nint hdcMonitor,
    Rect lprcMonitor,
    nint dwData
);
