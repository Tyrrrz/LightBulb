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
    public sealed class WindowsHotkeyService : WinApiServiceBase, IHotkeyService
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
        }

        protected override void WndProc(Message message)
        {
            if (message.Msg != 0x0312) return;

            int id = message.WParam.ToInt32();
            var handler = _hotkeyHandlerDic.GetOrDefault(id);

            handler?.Invoke();
        }

        /// <inheritdoc />
        public void Register(Hotkey hotkey, HotkeyHandler handler)
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
        public void Unregister(Hotkey hotkey)
        {
            int vk = KeyInterop.VirtualKeyFromKey(hotkey.Key);
            int mods = (int) hotkey.Modifiers;
            int id = (vk << 8) | mods;

            if (!UnregisterHotKeyInternal(SpongeHandle, id))
                Debug.WriteLine("Could not unregister a hotkey", GetType().Name);
            _hotkeyHandlerDic.Remove(id);
        }

        /// <inheritdoc />
        public void UnregisterAll()
        {
            foreach (int key in _hotkeyHandlerDic.Keys)
            {
                if (!UnregisterHotKeyInternal(SpongeHandle, key))
                    Debug.WriteLine("Could not unregister a hotkey", GetType().Name);
            }
            _hotkeyHandlerDic.Clear();
        }

        public override void Dispose()
        {
            UnregisterAll();
            base.Dispose();
        }
    }
}