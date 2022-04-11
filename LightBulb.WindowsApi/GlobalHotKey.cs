using System;
using System.Diagnostics;
using System.Threading;
using LightBulb.WindowsApi.Native;

namespace LightBulb.WindowsApi;

public partial class GlobalHotKey : IDisposable
{
    private readonly int _id;

    private readonly object _lock = new();
    private readonly IDisposable _wndProcRegistration;

    private DateTimeOffset _lastTriggerTimestamp = DateTimeOffset.MinValue;

    private GlobalHotKey(int id, Action callback)
    {
        _id = id;

        _wndProcRegistration = WndProc.Listen(WndProc.Ids.GlobalHotkeyMessage, m =>
        {
            // Filter out other hotkey events
            if (m.WParam.ToInt32() != _id)
                return;

            // Throttle triggers
            lock (_lock)
            {
                if ((DateTimeOffset.Now - _lastTriggerTimestamp).Duration().TotalSeconds < 0.05)
                    return;

                _lastTriggerTimestamp = DateTimeOffset.Now;
            }

            callback();
        });
    }

    ~GlobalHotKey() => Dispose();

    public void Dispose()
    {
        _wndProcRegistration.Dispose();

        if (!NativeMethods.UnregisterHotKey(WndProc.Handle, _id))
            Debug.WriteLine($"Failed to dispose global hotkey (ID: {_id}).");

        GC.SuppressFinalize(this);
    }
}

public partial class GlobalHotKey
{
    private static int _lastHotKeyId;

    public static GlobalHotKey? TryRegister(int virtualKey, int modifiers, Action callback)
    {
        var id = Interlocked.Increment(ref _lastHotKeyId);

        if (!NativeMethods.RegisterHotKey(WndProc.Handle, id, modifiers, virtualKey))
        {
            Debug.WriteLine($"Failed to register global hotkey (key: {virtualKey}, mods: {modifiers}).");
            return null;
        }

        return new GlobalHotKey(id, callback);
    }
}