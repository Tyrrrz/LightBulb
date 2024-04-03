﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using LightBulb.PlatformInterop.Internal;

namespace LightBulb.PlatformInterop;

public partial class NativeWindow(nint handle) : NativeResource(handle)
{
    public NativeProcess? TryGetProcess()
    {
        _ = NativeMethods.GetWindowThreadProcessId(Handle, out var processId);
        if (processId == 0)
        {
            Debug.WriteLine($"Failed to retrieve process ID for window #{Handle}.");
            return null;
        }

        return NativeProcess.TryGet((int)processId);
    }

    public Monitor? TryGetMonitor()
    {
        var monitorHandle = NativeMethods.MonitorFromWindow(Handle, 0);
        if (monitorHandle == 0)
        {
            Debug.WriteLine($"Failed to retrieve monitor for window #{Handle}.");
            return null;
        }

        return new Monitor(monitorHandle);
    }

    public Rect? TryGetRect() => NativeMethods.GetWindowRect(Handle, out var rect) ? rect : null;

    public Rect? TryGetClientRect() =>
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
        if (TryGetMonitor() is not { } monitor)
            return false;

        if (monitor.TryGetBounds() is not { } monitorRect)
            return false;

        if (TryGetRect() is not { } windowRect)
            return false;

        // Calculate absolute window client rect (not relative to the window)
        var windowClientRect = TryGetClientRect() ?? Rect.Empty;

        var absoluteWindowClientRect = new Rect(
            windowRect.Left + windowClientRect.Left,
            windowRect.Top + windowClientRect.Top,
            windowRect.Left + windowClientRect.Right,
            windowRect.Top + windowClientRect.Bottom
        );

        return absoluteWindowClientRect.Left <= monitorRect.Left
            && absoluteWindowClientRect.Top <= monitorRect.Top
            && absoluteWindowClientRect.Right >= monitorRect.Right
            && absoluteWindowClientRect.Bottom >= monitorRect.Bottom;
    }

    protected override void Dispose(bool disposing) { }
}

public partial class NativeWindow
{
    public static NativeWindow? TryGetForeground()
    {
        var handle = NativeMethods.GetForegroundWindow();
        if (handle == 0)
        {
            Debug.WriteLine("Failed to retrieve foreground window.");
            return null;
        }

        return new NativeWindow(handle);
    }

    public static IReadOnlyList<NativeWindow> GetAll()
    {
        var result = new List<NativeWindow>();

        if (
            !NativeMethods.EnumWindows(
                (hWnd, _) =>
                {
                    if (hWnd != 0)
                    {
                        var window = new NativeWindow(hWnd);
                        result.Add(window);
                    }

                    return true;
                },
                0
            )
        )
        {
            Debug.WriteLine("Failed to enumerate windows.");
        }

        return result;
    }
}
