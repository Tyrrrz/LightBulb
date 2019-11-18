using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using LightBulb.Models;
using LightBulb.WindowsApi;

namespace LightBulb.Services
{
    public class HotKeyService : IDisposable
    {
        private readonly List<GlobalHotKey> _registeredHotKeys = new List<GlobalHotKey>();

        public void RegisterHotKey(HotKey hotKey, Action handler)
        {
            // Get codes that represent virtual key and modifiers
            var virtualKey = KeyInterop.VirtualKeyFromKey(hotKey.Key);
            var modifiers = (int) hotKey.Modifiers;

            // Register hotkey
            var registeredHotKey = GlobalHotKey.Register(virtualKey, modifiers, handler);

            // Add to the list
            if (registeredHotKey != null)
                _registeredHotKeys.Add(registeredHotKey);
            else
                Debug.WriteLine("Failed to register hotkey.");
        }

        public void UnregisterAllHotKeys()
        {
            // Dispose all registered hotkeys and remove them from the lsit
            foreach (var registeredHotKey in _registeredHotKeys.ToArray())
            {
                registeredHotKey.Dispose();
                _registeredHotKeys.Remove(registeredHotKey);
            }
        }

        public void Dispose() => UnregisterAllHotKeys();
    }
}