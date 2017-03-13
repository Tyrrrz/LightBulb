using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Input;
using LightBulb.Models;
using LightBulb.Services.Abstract;
using LightBulb.Services.Interfaces;
using Tyrrrz.Extensions;

namespace LightBulb.Services
{
    public class WindowsHotkeyService : WinApiServiceBase, IHotkeyService
    {
        #region WinAPI

        [DllImport("user32.dll", EntryPoint = "RegisterHotKey", SetLastError = true)]
        private static extern bool RegisterHotKeyInternal(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll", EntryPoint = "UnregisterHotKey", SetLastError = true)]
        private static extern bool UnregisterHotKeyInternal(IntPtr hWnd, int id);

        #endregion

        private readonly Dictionary<int, HotkeyHandler> _hotkeyHandlerDic;

        public WindowsHotkeyService()
        {
            _hotkeyHandlerDic = new Dictionary<int, HotkeyHandler>();
            WndProced += ProcessMessage;
        }

        private void ProcessMessage(object sender, Message message)
        {
            if (message.Msg != 0x0312) return;

            int id = message.WParam.ToInt32();
            var handler = _hotkeyHandlerDic.GetOrDefault(id);

            handler?.Invoke();
        }

        /// <inheritdoc />
        public void RegisterHotkey(Hotkey hotkey, HotkeyHandler handler)
        {
            int vk = KeyInterop.VirtualKeyFromKey(hotkey.Key);
            int mods = (int) hotkey.Modifiers;
            int id = (vk << 8) | mods;

            if (!RegisterHotKeyInternal(SpongeHandle, id, mods, vk))
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
            int vk = KeyInterop.VirtualKeyFromKey(hotkey.Key);
            int mods = (int) hotkey.Modifiers;
            int id = (vk << 8) | mods;

            if (!UnregisterHotKeyInternal(SpongeHandle, id))
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
                if (!UnregisterHotKeyInternal(SpongeHandle, hotkey.Key))
                {
                    CheckLogWin32Error();
                    Debug.WriteLine("Could not unregister a hotkey", GetType().Name);
                }
            }
            _hotkeyHandlerDic.Clear();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                WndProced -= ProcessMessage;
            }
            UnregisterAllHotkeys();
        }
    }
}