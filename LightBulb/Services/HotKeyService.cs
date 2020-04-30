using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using LightBulb.Models;
using LightBulb.WindowsApi;
using Stylet;

namespace LightBulb.Services
{
    public class HotKeyService : IDisposable
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly List<GlobalHotKey> _registeredHotKeys = new List<GlobalHotKey>();

        public HotKeyService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void RegisterHotKey(HotKey hotKey, Action handler)
        {
            // Get codes that represent virtual key and modifiers
            var virtualKey = KeyInterop.VirtualKeyFromKey(hotKey.Key);
            var modifiers = (int) hotKey.Modifiers;

            // Register hotkey
            var registeredHotKey = GlobalHotKey.TryRegister(virtualKey, modifiers, handler);

            if (registeredHotKey != null)
                _registeredHotKeys.Add(registeredHotKey);
            else
                Debug.WriteLine("Failed to register hotkey.");
        }

        public void UnregisterAllHotKeys()
        {
            // Dispose all registered hotkeys and remove them from the list
            foreach (var registeredHotKey in _registeredHotKeys.ToArray())
            {
                registeredHotKey.Dispose();
                _registeredHotKeys.Remove(registeredHotKey);
            }
        }

        public void Dispose() => UnregisterAllHotKeys();
    }
}