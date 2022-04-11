using System;
using System.Diagnostics;
using LightBulb.WindowsApi.Native;

namespace LightBulb.WindowsApi;

public partial class SystemHook : IDisposable
{
    private readonly IntPtr _handle;

    // We only need the reference to the delegate to prevent it from
    // being garbage collected too early.
    // ReSharper disable once NotAccessedField.Local
    private readonly NativeMethods.WinEventProc _winEventProc;

    private SystemHook(IntPtr handle, NativeMethods.WinEventProc winEventProc)
    {
        _handle = handle;
        _winEventProc = winEventProc;
    }

    ~SystemHook() => Dispose();

    public void Dispose()
    {
        if (!NativeMethods.UnhookWinEvent(_handle))
            Debug.WriteLine($"Failed to dispose system hook (handle: {_handle}).");

        GC.SuppressFinalize(this);
    }
}

public partial class SystemHook
{
    public static int ForegroundWindowChanged => 3;

    public static SystemHook? TryRegister(int hookId, Action callback)
    {
        var proc = new NativeMethods.WinEventProc((_, _, _, idObject, _, _, _) =>
        {
            // Ignore events from non-windows
            if (idObject != 0)
                return;

            callback();
        });

        var handle = NativeMethods.SetWinEventHook(
            (uint) hookId,
            (uint) hookId,
            IntPtr.Zero,
            proc,
            0,
            0,
            0
        );

        if (handle == IntPtr.Zero)
        {
            Debug.WriteLine($"Failed to register system hook (ID: {hookId}).");
            return null;
        }

        return new SystemHook(handle, proc);
    }
}