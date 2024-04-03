using System;
using System.Runtime.InteropServices;
using System.Text;

namespace LightBulb.PlatformInterop.Internal;

internal static partial class NativeMethods
{
    private const string User32 = "user32.dll";

    [DllImport(User32, SetLastError = true)]
    public static extern bool EnumDisplayMonitors(
        nint hdc,
        nint lprcClip,
        EnumMonitorsProc lpfnEnum,
        nint dwData
    );

    [DllImport(User32, SetLastError = true)]
    public static extern nint MonitorFromWindow(nint hWnd, uint dwFlags);

    [DllImport(User32, SetLastError = true)]
    public static extern bool GetMonitorInfo(nint hMonitor, ref MonitorInfoEx lpmi);

    [DllImport(User32, SetLastError = true)]
    public static extern ushort RegisterClassEx(ref WndClassEx lpwcx);

    [DllImport(User32, SetLastError = true)]
    public static extern bool UnregisterClass(string lpClassName, IntPtr hInstance);

    [DllImport(User32, SetLastError = true)]
    public static extern nint CreateWindowEx(
        uint dwExStyle,
        string lpClassName,
        string lpWindowName,
        uint dwStyle,
        int x,
        int y,
        int nWidth,
        int nHeight,
        nint hWndParent,
        nint hMenu,
        nint hInstance,
        nint lpParam
    );

    [DllImport(User32, SetLastError = true)]
    public static extern bool DestroyWindow(nint hWnd);

    [DllImport(User32, SetLastError = true)]
    public static extern nint DefWindowProc(nint hWnd, uint msg, nint wParam, nint lParam);

    [DllImport(User32, SetLastError = true)]
    public static extern bool PostMessage(nint hWnd, uint msg, nint wParam, nint lParam);

    [DllImport(User32, SetLastError = true)]
    public static extern nint GetForegroundWindow();

    [DllImport(User32, SetLastError = true)]
    public static extern bool GetWindowRect(nint hWnd, out Rect lpRect);

    [DllImport(User32, SetLastError = true)]
    public static extern bool GetClientRect(nint hWnd, out Rect lpRect);

    [DllImport(User32, SetLastError = true)]
    public static extern bool IsWindowVisible(nint hWnd);

    [DllImport(User32, SetLastError = true)]
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

    [DllImport(User32, SetLastError = true)]
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
