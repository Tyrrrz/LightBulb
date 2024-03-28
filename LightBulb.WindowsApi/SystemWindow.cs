using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using LightBulb.WindowsApi.Types;

namespace LightBulb.WindowsApi;

public partial class SystemWindow : NativeResource
{
    private SystemWindow(nint handle)
        : base(handle) { }

    private Rect? TryGetRect() => NativeMethods.GetWindowRect(Handle, out var rect) ? rect : null;

    private Rect? TryGetClientRect() =>
        NativeMethods.GetClientRect(Handle, out var rect) ? rect : null;

    public string? TryGetClassName()
    {
        var buffer = new StringBuilder(256);

        return NativeMethods.GetClassName(Handle, buffer, buffer.Capacity) >= 0
            ? buffer.ToString()
            : null;
    }

    public bool IsSystemWindow()
    {
        var className = TryGetClassName();

        return string.Equals(className, "Progman", StringComparison.OrdinalIgnoreCase)
            || string.Equals(className, "WorkerW", StringComparison.OrdinalIgnoreCase)
            || string.Equals(className, "ImmersiveLauncher", StringComparison.OrdinalIgnoreCase)
            || string.Equals(className, "ImmersiveSwitchList", StringComparison.OrdinalIgnoreCase)
            || string.Equals(className, "MultitaskingViewFrame", StringComparison.OrdinalIgnoreCase)
            || string.Equals(className, "ForegroundStaging", StringComparison.OrdinalIgnoreCase)
            || string.Equals(
                className,
                "ApplicationManager_DesktopShellWindow",
                StringComparison.OrdinalIgnoreCase
            );
    }

    public bool IsVisible() => NativeMethods.IsWindowVisible(Handle);

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
        var screenRect = Screen.FromHandle(Handle).Bounds;

        return absoluteWindowClientRect.Left <= screenRect.Left
            && absoluteWindowClientRect.Top <= screenRect.Top
            && absoluteWindowClientRect.Right >= screenRect.Right
            && absoluteWindowClientRect.Bottom >= screenRect.Bottom;
    }

    public SystemProcess? TryGetProcess()
    {
        _ = NativeMethods.GetWindowThreadProcessId(Handle, out var processId);
        if (processId == 0)
        {
            Debug.WriteLine($"Failed to retrieve process ID for window #{Handle}.");
            return null;
        }

        return SystemProcess.TryOpen((int)processId);
    }

    protected override void Dispose(bool disposing)
    {
        // Doesn't actually need to be disposed, just here for consistency
    }
}

public partial class SystemWindow
{
    public static SystemWindow? TryGetForeground()
    {
        var handle = NativeMethods.GetForegroundWindow();
        if (handle == 0)
        {
            Debug.WriteLine("Failed to retrieve foreground window.");
            return null;
        }

        return new SystemWindow(handle);
    }

    public static IReadOnlyList<SystemWindow> GetAll()
    {
        var result = new List<SystemWindow>();

        bool EnumWindows(nint hWnd, nint _)
        {
            if (hWnd != 0)
            {
                var window = new SystemWindow(hWnd);
                result.Add(window);
            }

            return true;
        }

        if (!NativeMethods.EnumWindows(EnumWindows, 0))
            Debug.WriteLine("Failed to enumerate windows.");

        return result;
    }
}
