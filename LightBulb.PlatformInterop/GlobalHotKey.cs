using System;
using System.Diagnostics;
using System.Threading;

namespace LightBulb.PlatformInterop;

public partial class GlobalHotKey : NativeResource
{
    private readonly object _lock = new();
    private readonly IDisposable _wndProcRegistration;

    private DateTimeOffset _lastTriggerTimestamp = DateTimeOffset.MinValue;

    public GlobalHotKey(nint handle, Action callback)
        : base(handle)
    {
        _wndProcRegistration = WndProc.Listen(
            WndProc.Ids.GlobalHotkeyMessage,
            m =>
            {
                // Filter out other hotkey events
                if (m.WParam != Handle)
                    return;

                // Throttle triggers
                lock (_lock)
                {
                    if ((DateTimeOffset.Now - _lastTriggerTimestamp).Duration().TotalSeconds < 0.05)
                        return;

                    _lastTriggerTimestamp = DateTimeOffset.Now;
                }

                callback();
            }
        );
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _wndProcRegistration.Dispose();

        if (!NativeMethods.UnregisterHotKey(WndProc.Handle, (int)Handle))
            Debug.WriteLine($"Failed to dispose global hotkey #{Handle}.");
    }
}

public partial class GlobalHotKey
{
    private static int _lastHotKeyHandle;

    public static GlobalHotKey? TryRegister(int virtualKey, int modifiers, Action callback)
    {
        var handle = Interlocked.Increment(ref _lastHotKeyHandle);
        if (!NativeMethods.RegisterHotKey(WndProc.Handle, handle, modifiers, virtualKey))
        {
            Debug.WriteLine(
                $"Failed to register global hotkey (key: {virtualKey}, mods: {modifiers})."
            );

            return null;
        }

        return new GlobalHotKey(handle, callback);
    }
}
