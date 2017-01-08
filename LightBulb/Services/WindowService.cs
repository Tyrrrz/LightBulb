using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using LightBulb.Models;

namespace LightBulb.Services
{
    public class WindowService : WinApiServiceBase, IDisposable
    {
        private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild, uint dwEventThread,
            uint dwmsEventTime);

        [DllImport("user32.dll", EntryPoint = "GetForegroundWindow", SetLastError = true)]
        private static extern IntPtr GetForegroundWindowInternal();

        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow", SetLastError = true)]
        private static extern IntPtr GetDesktopWindowInternal();

        [DllImport("user32.dll", EntryPoint = "GetShellWindow", SetLastError = true)]
        private static extern IntPtr GetShellWindowInternal();

        [DllImport("user32.dll", EntryPoint = "GetWindowRect", SetLastError = true)]
        private static extern int GetWindowRectInternal(IntPtr hWindow, out Rect rect);

        [DllImport("user32.dll", EntryPoint = "SetWinEventHook", SetLastError = true)]
        private static extern IntPtr SetWinEventHookInternal(uint eventMin, uint eventMax,
            IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc,
            uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll", EntryPoint = "UnhookWinEvent", SetLastError = true)]
        private static extern bool UnhookWinEventInternal(IntPtr hWinEventHook);

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable (prevent garbage collection)
        private readonly WinEventDelegate _winEventHandler;
        private readonly IntPtr _foregroundWindowChangedHook;
        private readonly IntPtr _windowResizeHook;
        private bool _isFullScreen;

        public Settings Settings => Settings.Default;

        public bool IsFullScreen
        {
            get { return _isFullScreen; }
            private set
            {
                if (IsFullScreen != value)
                {
                    _isFullScreen = value;
                    FullScreenStateChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler FullScreenStateChanged;

        public WindowService()
        {
            // Hooks
            _winEventHandler =
                (hook, type, hwnd, idObject, child, thread, time) => Update();
            _foregroundWindowChangedHook = SetWinEventHookInternal(0x0003, 0x0003, IntPtr.Zero,
                _winEventHandler, 0, 0, 0);
            _windowResizeHook = SetWinEventHookInternal(0x800B, 0x800B, IntPtr.Zero,
                _winEventHandler, 0, 0, 0); // HACK: this raises too many events per second, need to optimize

            // Init
            Update();
        }

        private void Update()
        {
            IsFullScreen = IsWindowFullScreen(GetForegroundWindow());
        }

        public IntPtr GetForegroundWindow()
        {
            var result = GetForegroundWindowInternal();
            CheckThrowWin32Error();
            return result;
        }

        public IntPtr GetDesktopWindow()
        {
            var result = GetDesktopWindowInternal();
            CheckThrowWin32Error();
            return result;
        }

        public IntPtr GetShellWindow()
        {
            var result = GetShellWindowInternal();
            CheckThrowWin32Error();
            return result;
        }

        public Rect GetWindowRect(IntPtr hWindow)
        {
            Rect result;
            GetWindowRectInternal(hWindow, out result);
            CheckThrowWin32Error();
            return result;
        }

        public bool IsWindowFullScreen(IntPtr hWindow)
        {
            // Get foreground window
            var foreground = GetForegroundWindow();
            if (foreground == IntPtr.Zero) return false;

            // Get desktop and shell
            var desktop = GetDesktopWindow();
            var shell = GetShellWindow();

            // If foreground is desktop or shell - return
            if (foreground == desktop || foreground == shell) return false;

            // Get the window rect
            var windowRect = GetWindowRect(foreground);

            // If window rect has retarded values, it's most likely a fullscreen game
            if (windowRect.Left <= 0 && windowRect.Top <= 0 && windowRect.Right <= 0 && windowRect.Bottom <= 0)
                return true;

            // Get the screen rect and compare
            var screenRect = Screen.FromHandle(foreground).Bounds;
            return windowRect.Left <= 0 && windowRect.Top <= 0 &&
                   windowRect.Height >= screenRect.Height && windowRect.Width >= screenRect.Width;
        }

        public void Dispose()
        {
            UnhookWinEventInternal(_foregroundWindowChangedHook);
            UnhookWinEventInternal(_windowResizeHook);
        }
    }
}