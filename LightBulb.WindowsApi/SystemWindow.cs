using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using LightBulb.WindowsApi.Native;

namespace LightBulb.WindowsApi;

public partial class SystemWindow
{
    private readonly IntPtr _handle;

    private SystemWindow(IntPtr handle) => _handle = handle;

    private Rect? TryGetRect() =>
        NativeMethods.GetWindowRect(_handle, out var rect)
            ? rect
            : null;

    private Rect? TryGetClientRect() =>
        NativeMethods.GetClientRect(_handle, out var rect)
            ? rect
            : null;

    public string? TryGetClassName()
    {
        var buffer = new StringBuilder(256);

        return NativeMethods.GetClassName(_handle, buffer, buffer.Capacity) >= 0
            ? buffer.ToString()
            : null;
    }

    public bool IsSystemWindow()
    {
        var className = TryGetClassName();

        return
            string.Equals(className, "Progman", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(className, "WorkerW", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(className, "ImmersiveLauncher", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(className, "ImmersiveSwitchList", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(className, "MultitaskingViewFrame", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(className, "ForegroundStaging", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(className, "ApplicationManager_DesktopShellWindow", StringComparison.OrdinalIgnoreCase);
    }

    public bool IsVisible() => NativeMethods.IsWindowVisible(_handle);

    public bool IsFullScreen()
    {
        // Get window rect
        if (TryGetRect() is not { } windowRect)
            return false;

        // Calculate absolute window client rect (not relative to window)
        var windowClientRect = TryGetClientRect() ?? Rect.Empty;

        var absoluteWindowClientRect = new Rect(
            windowRect.Left + windowClientRect.Left,
            windowRect.Top + windowClientRect.Top,
            windowRect.Left + windowClientRect.Right,
            windowRect.Top + windowClientRect.Bottom
        );

        // Check if the window covers up screen bounds
        var screenRect = Screen.FromHandle(_handle).Bounds;

        return
            absoluteWindowClientRect.Left <= 0 &&
            absoluteWindowClientRect.Top <= 0 &&
            absoluteWindowClientRect.Right >= screenRect.Right &&
            absoluteWindowClientRect.Bottom >= screenRect.Bottom;
    }

    public SystemProcess? TryGetProcess()
    {
        _ = NativeMethods.GetWindowThreadProcessId(_handle, out var processId);

        if (processId == 0)
        {
            Debug.WriteLine($"Failed to retrieve process ID for window (handle: {_handle}).");
            return null;
        }

        return SystemProcess.TryOpen((int) processId);
    }
}

public partial class SystemWindow
{
    public static SystemWindow? TryGetForegroundWindow()
    {
        var handle = NativeMethods.GetForegroundWindow();

        if (handle == IntPtr.Zero)
        {
            Debug.WriteLine("Failed to retrieve foreground window.");
            return null;
        }

        return new SystemWindow(handle);
    }

    public static IReadOnlyList<SystemWindow> GetAllWindows()
    {
        var result = new List<SystemWindow>();

        bool EnumWindows(IntPtr hWnd, IntPtr _)
        {
            if (hWnd != IntPtr.Zero)
            {
                var window = new SystemWindow(hWnd);
                result.Add(window);
            }

            return true;
        }

        if (!NativeMethods.EnumWindows(EnumWindows, IntPtr.Zero))
            Debug.WriteLine("Failed to enumerate windows.");

        return result;
    }
}