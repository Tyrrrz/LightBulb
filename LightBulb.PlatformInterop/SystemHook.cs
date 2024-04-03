﻿using System;
using System.Diagnostics;
using LightBulb.PlatformInterop.Internal;

namespace LightBulb.PlatformInterop;

public partial class SystemHook(nint handle, Delegate winEventProc) : NativeResource(handle)
{
    // We only need the reference to the delegate to prevent it from being garbage collected too early
    // ReSharper disable once UnusedMember.Local
    private readonly Delegate _winEventProc = winEventProc;

    protected override void Dispose(bool disposing)
    {
        if (!NativeMethods.UnhookWinEvent(Handle))
            Debug.WriteLine($"Failed to dispose system hook #{Handle}.");
    }
}

public partial class SystemHook
{
    public static SystemHook? TryRegister(int hookId, Action callback)
    {
        var proc = new WinEventProc((_, _, _, _, _, _, _) => callback());

        var handle = NativeMethods.SetWinEventHook((uint)hookId, (uint)hookId, 0, proc, 0, 0, 0);
        if (handle == 0)
        {
            Debug.WriteLine($"Failed to register system hook #{hookId}.");
            return null;
        }

        return new SystemHook(handle, proc);
    }
}

public partial class SystemHook
{
    public static class Ids
    {
        public static int ForegroundWindowChanged => 3;
    }
}
