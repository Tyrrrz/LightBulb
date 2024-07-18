using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia.Input;
using Avalonia.Win32.Input;
using LightBulb.Models;
using LightBulb.PlatformInterop;
using LightBulb.Utils.Extensions;

namespace LightBulb.Services;

public class HotKeyService : IDisposable
{
    private readonly List<GlobalHotKey> _hotKeyRegistrations = [];

    public void RegisterHotKey(HotKey hotKey, Action callback)
    {
        // Convert Avalonia key/modifiers to Windows API virtual key/modifiers
        var virtualKey = KeyInterop.VirtualKeyFromKey(hotKey.Key.ToQwertyKey());
        var modifiers = (int)hotKey.Modifiers;

        var registration = GlobalHotKey.TryRegister(virtualKey, modifiers, callback);

        if (registration is not null)
            _hotKeyRegistrations.Add(registration);
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
