using System;
using System.Runtime.InteropServices;
using System.Text;

namespace LightBulb.WindowsApi.Native;

internal static partial class NativeMethods
{
    private const string User32 = "user32.dll";

    [DllImport(User32, SetLastError = true)]
    public static extern IntPtr GetForegroundWindow();

    [DllImport(User32, SetLastError = true)]
    public static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);

    [DllImport(User32, SetLastError = true)]
    public static extern bool GetClientRect(IntPtr hWnd, out Rect lpRect);

    [DllImport(User32, SetLastError = true)]
    public static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport(User32, CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetClassName(
        IntPtr hWnd,
        StringBuilder lpClassName,
        int nMaxCount
    );

    [DllImport(User32, SetLastError = true)]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [DllImport(User32, SetLastError = true)]
    public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport(User32, SetLastError = true)]
    public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

    [DllImport(User32, SetLastError = true)]
    public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport(User32, SetLastError = true)]
    public static extern IntPtr RegisterPowerSettingNotification(
        IntPtr hRecipient,
        Guid powerSettingGuid,
        int flags
    );

    [DllImport(User32, SetLastError = true)]
    public static extern bool UnregisterPowerSettingNotification(IntPtr handle);

    public delegate void WinEventProc(
        IntPtr hWinEventHook,
        uint idEvent,
        IntPtr hWnd,
        int idObject,
        int idChild,
        uint idEventThread,
        uint dwmsEventTime
    );

    [DllImport(User32, SetLastError = true)]
    public static extern IntPtr SetWinEventHook(
        uint eventMin,
        uint eventMax,
        IntPtr hmodWinEventProc,
        WinEventProc pfnWinEventProc,
        uint idProcess,
        uint idThread,
        uint dwFlags
    );

    [DllImport(User32, SetLastError = true)]
    public static extern bool UnhookWinEvent(IntPtr hWinEventHook);
}