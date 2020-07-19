using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using LightBulb.Models;
using LightBulb.WindowsApi.Events;

namespace LightBulb.Services
{
    public class HotKeyService : IDisposable
    {
        private readonly List<IDisposable> _systemEventRegistrations = new List<IDisposable>();

        public void RegisterHotKey(HotKey hotKey, Action handler)
        {
            // Get codes that represent virtual key and modifiers
            var virtualKey = KeyInterop.VirtualKeyFromKey(hotKey.Key);
            var modifiers = (int) hotKey.Modifiers;

            var registration = SystemEvent.TryRegisterHotKey(virtualKey, modifiers, handler);

            if (registration != null)
                _systemEventRegistrations.Add(registration);
            else
                Debug.WriteLine("Failed to register hotkey.");
        }

        public void UnregisterAllHotKeys()
        {
            foreach (var registration in _systemEventRegistrations.ToArray())
            {
                registration.Dispose();
                _systemEventRegistrations.Remove(registration);
            }
        }

        public void Dispose() => UnregisterAllHotKeys();
    }
}