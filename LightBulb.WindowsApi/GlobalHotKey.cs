using System;
using System.Diagnostics;
using LightBulb.WindowsApi.Native;

namespace LightBulb.WindowsApi;

public partial class GlobalHotKey : IDisposable
{
    private readonly object _lock = new();
    private readonly IDisposable _wndProcRegistration;

    private DateTimeOffset _lastTriggerTimestamp = DateTimeOffset.MinValue;

    public int Id { get; }

    public int VirtualKey { get; }

    public int Modifiers { get; }

    public Action Callback { get; }

    public GlobalHotKey(int id, int virtualKey, int modifiers, Action callback)
    {
        Id = id;
        VirtualKey = virtualKey;
        Modifiers = modifiers;
        Callback = callback;

        _wndProcRegistration = WndProc.Listen(786, m =>
        {
            // Filter out other hotkey events
            if (m.WParam.ToInt32() != Id)
                return;

            // Throttle triggers
            lock (_lock)
            {
                if ((DateTimeOffset.Now - _lastTriggerTimestamp).Duration().TotalSeconds < 0.05)
                    return;

                _lastTriggerTimestamp = DateTimeOffset.Now;
            }

            Callback();
        });
    }

    ~GlobalHotKey() => Dispose();

    public void Dispose()
    {
        _wndProcRegistration.Dispose();

        if (!NativeMethods.UnregisterHotKey(WndProc.Handle, Id))
            Debug.WriteLine("Could not dispose global hotkey.");

        GC.SuppressFinalize(this);
    }
}

public partial class GlobalHotKey
{
    private static int _lastHotKeyId;

    public static GlobalHotKey? TryRegister(int virtualKey, int modifiers, Action callback)
    {
        var id = _lastHotKeyId++;
        return NativeMethods.RegisterHotKey(WndProc.Handle, id, modifiers, virtualKey)
            ? new GlobalHotKey(id, virtualKey, modifiers, callback)
            : null;
    }
}