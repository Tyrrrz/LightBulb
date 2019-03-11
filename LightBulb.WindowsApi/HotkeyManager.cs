using System;
using System.Collections.Generic;
using System.Linq;
using LightBulb.WindowsApi.Internal;
using Tyrrrz.Extensions;

namespace LightBulb.WindowsApi
{
    public class HotkeyManager : IDisposable
    {
        private readonly Dictionary<int, Action> _hotKeyHandlersMap = new Dictionary<int, Action>();
        private readonly WndProcSpongeWindow _wndProcSponge;

        private int _lastHotKeyId;

        public HotkeyManager()
        {
            _wndProcSponge = new WndProcSpongeWindow(m =>
            {
                // Only hotkey-related messages
                if (m.Msg != 0x0312)
                    return;

                // Get hotkey ID
                var id = m.WParam.ToInt32();

                // Process hotkey press
                _hotKeyHandlersMap.GetOrDefault(id)?.Invoke();
            });
        }

        ~HotkeyManager()
        {
            Dispose();
        }

        public void RegisterHotKey(int virtualKey, int modifiers, Action handler)
        {
            var id = _lastHotKeyId++;

            NativeMethods.RegisterHotKey(_wndProcSponge.Handle, id, modifiers, virtualKey);
            _hotKeyHandlersMap[id] = handler;
        }

        public void UnregisterAllHotKeys()
        {
            // Get all hotkey IDs
            var ids = _hotKeyHandlersMap.Keys.ToArray();

            // Clear hotkey handler map
            _hotKeyHandlersMap.Clear();

            // Unregister all hotkeys
            foreach (var id in ids)
                NativeMethods.UnregisterHotKey(_wndProcSponge.Handle, id);
        }

        public void Dispose()
        {
            UnregisterAllHotKeys();
            GC.SuppressFinalize(this);
        }
    }
}