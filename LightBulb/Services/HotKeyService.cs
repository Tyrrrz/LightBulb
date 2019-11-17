using System;
using System.Collections.Generic;
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
            var virtualKey = KeyInterop.VirtualKeyFromKey(hotKey.Key);
            var modifiers = (int) hotKey.Modifiers;

            var registeredHotKey = GlobalHotKey.Register(virtualKey, modifiers, handler);
            _registeredHotKeys.Add(registeredHotKey);
        }

        public void UnregisterAllHotKeys()
        {
            foreach (var registeredHotKey in _registeredHotKeys.ToArray())
            {
                registeredHotKey.Dispose();
                _registeredHotKeys.Remove(registeredHotKey);
            }
        }

        public void Dispose() => UnregisterAllHotKeys();
    }
}