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
        private WinEventDelegate _foregroundWindowChangedEventHandler;
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable (prevent garbage collection)
        private WinEventDelegate _foregroundWindowLocationChangedEventHandler;
        private IntPtr _foregroundWindowChangedHook;
        private IntPtr _foregroundWindowLocationChangedHook;

        private bool _useEventHooks;
        private bool _isForegroundFullScreen;
        private IntPtr _lastForegroundWindow;
        private uint _lastEventTime;

        public Settings Settings => Settings.Default;

        /// <summary>
        /// Enables or disables the event hooks
        /// </summary>
        public bool UseEventHooks
        {
            get { return _useEventHooks; }
            set
            {
                if (UseEventHooks == value) return;

                _useEventHooks = value;
                if (value)
                    InstallHooks();
                else
                    UninstallHooks();
            }
        }

        /// <summary>
        /// Gets whether the foreground window is fullscreen
        /// </summary>
        public bool IsForegroundFullScreen
        {
            get { return _isForegroundFullScreen; }
            private set
            {
                if (IsForegroundFullScreen == value) return;

                _isForegroundFullScreen = value;
                FullScreenStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Triggers when the foreground window has entered (or exited from) full screen mode
        /// </summary>
        public event EventHandler FullScreenStateChanged;

        public WindowService()
        {
            // Init
            IsForegroundFullScreen = IsWindowFullScreen(GetForegroundWindow());
        }

        private void InstallHooks()
        {
            _foregroundWindowChangedEventHandler =
                (hook, type, hwnd, idObject, child, thread, time) =>
                {
                    if (idObject != 0) return; // only events from windows
                    if (_lastEventTime == time) return; // skip duplicate events
                    _lastEventTime = time;

                    _lastForegroundWindow = hwnd;
                    IsForegroundFullScreen = IsWindowFullScreen(hwnd);

                    // Hook location changed event for foreground window
                    if (_foregroundWindowLocationChangedHook != IntPtr.Zero)
                        UnhookWinEventInternal(_foregroundWindowLocationChangedHook);
                    _foregroundWindowLocationChangedHook = SetWinEventHookInternal(0x800B, 0x800B, IntPtr.Zero,
                        _foregroundWindowLocationChangedEventHandler, 0, thread, 0);
                };
            _foregroundWindowLocationChangedEventHandler =
                (hook, type, hwnd, idObject, child, thread, time) =>
                {
                    if (idObject != 0) return; // only events from windows
                    if (hwnd != _lastForegroundWindow) return; // skip non-foregrond windows
                    if (_lastEventTime == time) return; // skip duplicate events
                    _lastEventTime = time;

                    IsForegroundFullScreen = IsWindowFullScreen(hwnd);
                };

            _foregroundWindowChangedHook = SetWinEventHookInternal(0x0003, 0x0003, IntPtr.Zero,
                _foregroundWindowChangedEventHandler, 0, 0, 0);
        }

        private void UninstallHooks()
        {
            UnhookWinEventInternal(_foregroundWindowChangedHook);
            UnhookWinEventInternal(_foregroundWindowLocationChangedHook);
        }

        /// <summary>
        /// Gets the window handle for the current foreground window
        /// </summary>
        public IntPtr GetForegroundWindow()
        {
            var result = GetForegroundWindowInternal();
            CheckThrowWin32Error();
            return result;
        }

        /// <summary>
        /// Gets the window handle for the desktop window
        /// </summary>
        public IntPtr GetDesktopWindow()
        {
            var result = GetDesktopWindowInternal();
            CheckThrowWin32Error();
            return result;
        }

        /// <summary>
        /// Gets the window handle for the shell window
        /// </summary>
        public IntPtr GetShellWindow()
        {
            var result = GetShellWindowInternal();
            CheckThrowWin32Error();
            return result;
        }

        /// <summary>
        /// Gets the rectangle bounds of the given window
        /// </summary>
        public Rect GetWindowRect(IntPtr hWindow)
        {
            Rect result;
            GetWindowRectInternal(hWindow, out result);
            CheckThrowWin32Error();
            return result;
        }

        /// <summary>
        /// Determines if the given window is running fullscreen
        /// </summary>
        public bool IsWindowFullScreen(IntPtr hWindow)
        {
            if (hWindow == IntPtr.Zero) return false;

            // Get desktop and shell
            var desktop = GetDesktopWindow();
            var shell = GetShellWindow();

            // If foreground is desktop or shell - return
            if (hWindow == desktop || hWindow == shell) return false;

            // Get the window rect
            var windowRect = GetWindowRect(hWindow);

            // If window rect has retarded values, it's most likely a fullscreen game
            if (windowRect.Left <= 0 && windowRect.Top <= 0 && windowRect.Right <= 0 && windowRect.Bottom <= 0)
                return true;

            // Get the screen rect and compare
            var screenRect = Screen.FromHandle(hWindow).Bounds;
            return windowRect.Left <= 0 && windowRect.Top <= 0 &&
                   windowRect.Right >= screenRect.Right && windowRect.Bottom >= screenRect.Bottom;
        }

        public void Dispose()
        {
            UninstallHooks();
        }
    }
}