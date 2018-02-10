using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using LightBulb.Internal;
using LightBulb.Models;
using Tyrrrz.Extensions;

namespace LightBulb.Services
{
    public class WindowsHotkeyService : IHotkeyService, IDisposable
    {
        private readonly SpongeWindow _sponge;
        private readonly Dictionary<int, HotkeyHandler> _hotkeyHandlerDic;

        public WindowsHotkeyService()
        {
            _sponge = new SpongeWindow();
            _hotkeyHandlerDic = new Dictionary<int, HotkeyHandler>();

            _sponge.WndProcFired += ProcessMessage;
        }

        ~WindowsHotkeyService()
        {
            Dispose(false);
        }

        private void ProcessMessage(object sender, WndProcEventArgs args)
        {
            if (args.Message.Msg != 0x0312) return;

            var id = args.Message.WParam.ToInt32();
            var handler = _hotkeyHandlerDic.GetOrDefault(id);

            handler?.Invoke();
        }

        /// <inheritdoc />
        public void RegisterHotkey(Hotkey hotkey, HotkeyHandler handler)
        {
            var vk = KeyInterop.VirtualKeyFromKey((Key) hotkey.Key);
            var mods = hotkey.Modifiers;
            var id = (vk << 8) | mods;

            if (!NativeMethods.RegisterHotKey(_sponge.Handle, id, mods, vk))
            {
                Debug.WriteLine("Could not register a hotkey", GetType().Name);
                return;
            }

            _hotkeyHandlerDic.Add(id, handler);
        }

        /// <inheritdoc />
        public void UnregisterHotkey(Hotkey hotkey)
        {
            var vk = KeyInterop.VirtualKeyFromKey((Key) hotkey.Key);
            var mods = hotkey.Modifiers;
            var id = (vk << 8) | mods;

            if (!NativeMethods.UnregisterHotKey(_sponge.Handle, id))
            {
                Debug.WriteLine("Could not unregister a hotkey", GetType().Name);
            }

            _hotkeyHandlerDic.Remove(id);
        }

        /// <inheritdoc />
        public void UnregisterAllHotkeys()
        {
            foreach (var hotkey in _hotkeyHandlerDic)
            {
                if (!NativeMethods.UnregisterHotKey(_sponge.Handle, hotkey.Key))
                {
                    Debug.WriteLine("Could not unregister a hotkey", GetType().Name);
                }
            }

            _hotkeyHandlerDic.Clear();
        }

        protected void Dispose(bool disposing)
        {
            UnregisterAllHotkeys();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}