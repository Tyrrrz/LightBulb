using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
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
        private readonly WindowInteropHelper _host;

        public WindowsHotkeyService()
        {
            _hotkeyHandlerDic = new Dictionary<int, HotkeyHandler>();
            _host = new WindowInteropHelper(Application.Current.MainWindow);
            ComponentDispatcher.ThreadPreprocessMessage += PreprocessMessage;
        }

        private void PreprocessMessage(ref MSG msg, ref bool handled)
        {
            if (msg.message != 0x0312) return;

            int id = msg.wParam.ToInt32();
            var handler = _hotkeyHandlerDic.GetOrDefault(id);

            handler?.Invoke();
        }

        private bool TryGetFreeId(out int id)
        {
            id = int.MinValue;
            if (!_hotkeyHandlerDic.Any()) return true;
            foreach (int usedKey in _hotkeyHandlerDic.Keys)
            {
                if (usedKey > id) return true;
                id++;
            }
            return false;
        }

        public void Register(Key key, ModifierKeys modifiers, HotkeyHandler handler)
        {
            int vk = KeyInterop.VirtualKeyFromKey(key);
            int mods = (int) modifiers;
            int id;

            if (!TryGetFreeId(out id))
            {
                Debug.WriteLine("Could not find a free id for a hotkey", GetType().Name);
                return;
            }

            if (!RegisterHotKeyInternal(_host.Handle, id, mods, vk))
            {
                CheckLogWin32Error();
                Debug.WriteLine("Could not register a hotkey", GetType().Name);
                return;
            }

            _hotkeyHandlerDic.Add(id, handler);
        }

        public void UnregisterAll()
        {
            foreach (int key in _hotkeyHandlerDic.Keys)
            {
                if (!UnregisterHotKeyInternal(_host.Handle, key))
                    Debug.WriteLine("Could not unregister a hotkey", GetType().Name);
            }
            _hotkeyHandlerDic.Clear();
        }

        public override void Dispose()
        {
            UnregisterAll();
            ComponentDispatcher.ThreadPreprocessMessage -= PreprocessMessage;
            base.Dispose();
        }
    }
}