namespace LightBulb.PlatformInterop;

internal delegate void WinEventProc(
    nint hWinEventHook,
    uint idEvent,
    nint hWnd,
    int idObject,
    int idChild,
    uint idEventThread,
    uint dwmsEventTime
);
