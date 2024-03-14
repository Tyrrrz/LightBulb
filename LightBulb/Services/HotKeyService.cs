using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using LightBulb.Models;
using LightBulb.Utils.Extensions;
using LightBulb.WindowsApi;

namespace LightBulb.Services;

public class HotKeyService : IDisposable
{
    private readonly List<GlobalHotKey> _hotKeyRegistrations = [];

    public void RegisterHotKey(HotKey hotKey, Action callback)
    {
        // Convert WPF key/modifiers to Windows API virtual key/modifiers
        var virtualKey = KeyInterop.VirtualKeyFromKey(hotKey.Key);
        var modifiers = (int)hotKey.Modifiers;

        var hotKeyRegistration = GlobalHotKey.TryRegister(virtualKey, modifiers, callback);

        if (hotKeyRegistration is not null)
            _hotKeyRegistrations.Add(hotKeyRegistration);
        else
            Debug.WriteLine("Failed to register hotkey.");
    }

    public void UnregisterAllHotKeys()
    {
        _hotKeyRegistrations.DisposeAll();
        _hotKeyRegistrations.Clear();
    }

    public void Dispose() => UnregisterAllHotKeys();
}
