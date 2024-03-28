using System;
using System.Runtime.InteropServices;
using System.Text;

namespace LightBulb.WindowsApi.Native;

internal static partial class NativeMethods
{
    private const string User32 = "user32.dll";

    [DllImport(User32, SetLastError = true)]
    public static extern bool GetMonitorInfo(nint hMonitor, out MonitorInfo lpmi);

    [DllImport(User32, SetLastError = true)]
    public static extern nint MonitorFromWindow(nint hWnd, uint dwFlags);

    [DllImport(User32, SetLastError = true)]
    public static extern bool EnumDisplayMonitors(
        nint hdc,
        nint lprcClip,
        EnumMonitorsProc lpfnEnum,
        nint dwData
    );

    [DllImport(User32, SetLastError = true)]
    public static extern nint GetForegroundWindow();

    [DllImport(User32, SetLastError = true)]
    public static extern bool GetWindowRect(nint hWnd, out Rect lpRect);

    [DllImport(User32, SetLastError = true)]
    public static extern bool GetClientRect(nint hWnd, out Rect lpRect);

    [DllImport(User32, SetLastError = true)]
    public static extern bool IsWindowVisible(nint hWnd);

    [DllImport(User32, CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetClassName(nint hWnd, StringBuilder lpClassName, int nMaxCount);

    [DllImport(User32, SetLastError = true)]
    public static extern uint GetWindowThreadProcessId(nint hWnd, out uint lpdwProcessId);

    [DllImport(User32, SetLastError = true)]
    public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, nint lParam);

    [DllImport(User32, SetLastError = true)]
    public static extern nint SetWinEventHook(
        uint eventMin,
        uint eventMax,
        nint hmodWinEventProc,
        WinEventProc pfnWinEventProc,
        uint idProcess,
        uint idThread,
        uint dwFlags
    );

    [DllImport(User32, SetLastError = true)]
    public static extern bool UnhookWinEvent(nint hWinEventHook);

    [DllImport(User32, CharSet = CharSet.Auto)]
    public static extern int MessageBox(nint hWnd, string text, string caption, uint type);

    [DllImport(User32, SetLastError = true)]
    public static extern bool RegisterHotKey(nint hWnd, int id, int fsModifiers, int vk);

    [DllImport(User32, SetLastError = true)]
    public static extern bool UnregisterHotKey(nint hWnd, int id);

    [DllImport(User32, SetLastError = true)]
    public static extern nint RegisterPowerSettingNotification(
        nint hRecipient,
        Guid powerSettingGuid,
        int flags
    );

    [DllImport(User32, SetLastError = true)]
    public static extern bool UnregisterPowerSettingNotification(nint handle);
}
