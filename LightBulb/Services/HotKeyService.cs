using System;
using System.Windows.Input;
using LightBulb.Models;
using LightBulb.WindowsApi;

namespace LightBulb.Services
{
    public class HotKeyService : IDisposable
    {
        private readonly HotkeyManager _hotkeyManager = new HotkeyManager();

        public void RegisterHotKey(HotKey hotKey, Action handler)
        {
            var virtualKey = KeyInterop.VirtualKeyFromKey(hotKey.Key);
            var modifiers = (int) hotKey.Modifiers;

            _hotkeyManager.RegisterHotKey(virtualKey, modifiers, handler);
        }

        public void UnregisterAllHotKeys() => _hotkeyManager.UnregisterAllHotKeys();

        public void Dispose() => _hotkeyManager.Dispose();
    }
}