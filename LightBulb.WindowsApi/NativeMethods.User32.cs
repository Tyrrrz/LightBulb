using System;
using System.Runtime.InteropServices;
using System.Text;
using LightBulb.WindowsApi.Types;

namespace LightBulb.WindowsApi;

internal static partial class NativeMethods
{
    private const string User32 = "user32.dll";

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

    public delegate bool EnumWindowsProc(nint hWnd, nint lParam);

    [DllImport(User32, SetLastError = true)]
    public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, nint lParam);

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

    public delegate void WinEventProc(
        nint hWinEventHook,
        uint idEvent,
        nint hWnd,
        int idObject,
        int idChild,
        uint idEventThread,
        uint dwmsEventTime
    );

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
}
