using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using LightBulb.Internal;
using LightBulb.Models;
using Tyrrrz.Extensions;

namespace LightBulb.Services
{
    public class WindowsHotkeyService : WinApiServiceBase, IHotkeyService
    {
        private readonly Dictionary<int, HotkeyHandler> _hotkeyHandlerDic;

        public WindowsHotkeyService()
        {
            _hotkeyHandlerDic = new Dictionary<int, HotkeyHandler>();
            WndProced += ProcessMessage;
        }

        private void ProcessMessage(object sender, WndProcEventArgs args)
        {
            if (args.Message.Msg != 0x0312) return;

            int id = args.Message.WParam.ToInt32();
            var handler = _hotkeyHandlerDic.GetOrDefault(id);

            handler?.Invoke();
        }

        /// <inheritdoc />
        public void RegisterHotkey(Hotkey hotkey, HotkeyHandler handler)
        {
            int vk = KeyInterop.VirtualKeyFromKey((Key) hotkey.Key);
            int mods = hotkey.Modifiers;
            int id = (vk << 8) | mods;

            if (!NativeMethods.RegisterHotKeyInternal(SpongeHandle, id, mods, vk))
            {
                CheckLogWin32Error();
                Debug.WriteLine("Could not register a hotkey", GetType().Name);
                return;
            }

            _hotkeyHandlerDic.Add(id, handler);
        }

        /// <inheritdoc />
        public void UnregisterHotkey(Hotkey hotkey)
        {
            int vk = KeyInterop.VirtualKeyFromKey((Key) hotkey.Key);
            int mods = hotkey.Modifiers;
            int id = (vk << 8) | mods;

            if (!NativeMethods.UnregisterHotKeyInternal(SpongeHandle, id))
            {
                CheckLogWin32Error();
                Debug.WriteLine("Could not unregister a hotkey", GetType().Name);
            }
            _hotkeyHandlerDic.Remove(id);
        }

        /// <inheritdoc />
        public void UnregisterAllHotkeys()
        {
            foreach (var hotkey in _hotkeyHandlerDic)
            {
                if (!NativeMethods.UnregisterHotKeyInternal(SpongeHandle, hotkey.Key))
                {
                    CheckLogWin32Error();
                    Debug.WriteLine("Could not unregister a hotkey", GetType().Name);
                }
            }
            _hotkeyHandlerDic.Clear();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                WndProced -= ProcessMessage;
            }
            UnregisterAllHotkeys();
            base.Dispose(disposing);
        }
    }
}