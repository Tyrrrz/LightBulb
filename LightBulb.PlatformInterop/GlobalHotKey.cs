using System;
using System.Diagnostics;
using System.Threading;
using LightBulb.PlatformInterop.Internal;

namespace LightBulb.PlatformInterop;

public partial class GlobalHotKey : NativeResource<int>
{
    private readonly object _lock = new();
    private readonly IDisposable _wndProcRegistration;

    private DateTimeOffset _lastTriggerTimestamp = DateTimeOffset.MinValue;

    public GlobalHotKey(int id, Action callback)
        : base(id)
    {
        _wndProcRegistration = WndProcSponge
            .Default
            .Listen(
                0x312,
                m =>
                {
                    // Filter out other hotkey events
                    if (m.WParam != Handle)
                        return;

                    // Throttle triggers
                    lock (_lock)
                    {
                        if (
                            (DateTimeOffset.Now - _lastTriggerTimestamp).Duration().TotalSeconds
                            < 0.05
                        )
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

        if (!NativeMethods.UnregisterHotKey(WndProcSponge.Default.Handle, Handle))
            Debug.WriteLine($"Failed to dispose global hotkey #{Handle}.");
    }
}

public partial class GlobalHotKey
{
    private static int _lastHotKeyHandle;

    public static GlobalHotKey? TryRegister(int virtualKey, int modifiers, Action callback)
    {
        var handle = Interlocked.Increment(ref _lastHotKeyHandle);
        
        if (
            !NativeMethods.RegisterHotKey(
                WndProcSponge.Default.Handle,
                handle,
                modifiers,
                virtualKey
            )
        )
        {
            Debug.WriteLine(
                $"Failed to register global hotkey (key: {virtualKey}, mods: {modifiers})."
            );

            return null;
        }

        return new GlobalHotKey(handle, callback);
    }
}
