using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using LightBulb.Models;
using LightBulb.Services.Abstract;

namespace LightBulb.Services
{
    public class WindowService : WinApiServiceBase, IDisposable
    {
        #region WinAPI
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
        #endregion

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable (prevent garbage collection)
        private WinEventDelegate _foregroundWindowChangedEventHandler;
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable (prevent garbage collection)
        private WinEventDelegate _foregroundWindowLocationChangedEventHandler;
        private IntPtr _foregroundWindowChangedHook;
        private IntPtr _foregroundWindowLocationChangedHook;

        private bool _isForegroundFullScreen;
        private IntPtr _lastForegroundWindow;

        private bool AreEventHooksEnabled
        {
            get
            {
                return
                    _foregroundWindowChangedHook != IntPtr.Zero ||
                    _foregroundWindowLocationChangedHook != IntPtr.Zero;
            }
            set
            {
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

        public Settings Settings => Settings.Default;

        /// <summary>
        /// Triggers when the foreground window has entered (or exited from) full screen mode
        /// </summary>
        public event EventHandler FullScreenStateChanged;

        public WindowService()
        {
            // Settings
            Settings.PropertyChanged += (sender, args) =>
            {
                UpdateConfiguration();
            };
            UpdateConfiguration();

            // Init
            IsForegroundFullScreen = IsWindowFullScreen(GetForegroundWindow());
        }

        private void UpdateConfiguration()
        {
            AreEventHooksEnabled = Settings.IsFullscreenBlocking;
        }

        private void InstallHooks()
        {
            if (AreEventHooksEnabled) return;

            _foregroundWindowChangedEventHandler =
                (hook, type, hwnd, idObject, child, thread, time) =>
                {
                    if (idObject != 0) return; // only events from windows

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
                    if (hwnd != _lastForegroundWindow) return; // skip non-foreground windows

                    IsForegroundFullScreen = IsWindowFullScreen(hwnd);
                };

            _foregroundWindowChangedHook = SetWinEventHookInternal(0x0003, 0x0003, IntPtr.Zero,
                _foregroundWindowChangedEventHandler, 0, 0, 0);

            Debug.WriteLine("Installed WinAPI hooks", GetType().Name);
        }

        private void UninstallHooks()
        {
            if (!AreEventHooksEnabled) return;

            UnhookWinEventInternal(_foregroundWindowChangedHook);
            UnhookWinEventInternal(_foregroundWindowLocationChangedHook);

            Debug.WriteLine("Uninstalled WinAPI hooks", GetType().Name);
        }

        /// <summary>
        /// Gets the window handle for the current foreground window
        /// </summary>
        public IntPtr GetForegroundWindow()
        {
            var result = GetForegroundWindowInternal();
            CheckLogWin32Error();
            return result;
        }

        /// <summary>
        /// Gets the window handle for the desktop window
        /// </summary>
        public IntPtr GetDesktopWindow()
        {
            var result = GetDesktopWindowInternal();
            CheckLogWin32Error();
            return result;
        }

        /// <summary>
        /// Gets the window handle for the shell window
        /// </summary>
        public IntPtr GetShellWindow()
        {
            var result = GetShellWindowInternal();
            CheckLogWin32Error();
            return result;
        }

        /// <summary>
        /// Gets the rectangle bounds of the given window
        /// </summary>
        public Rect GetWindowRect(IntPtr hWindow)
        {
            Rect result;
            GetWindowRectInternal(hWindow, out result);
            CheckLogWin32Error();
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

            // If window doesn't have a rect - return
            if (windowRect.Left == 0 && windowRect.Top == 0 && windowRect.Right == 0 && windowRect.Bottom == 0)
                return false;

            // If window rect has retarded values, it's most likely a fullscreen game
            if (windowRect.Left < 0 && windowRect.Top < 0 && windowRect.Right < 0 && windowRect.Bottom < 0)
                return true;

            // Get the screen rect and compare
            var screenRect = Screen.FromHandle(hWindow).Bounds;
            return windowRect.Left <= 0 && windowRect.Top <= 0 &&
                   windowRect.Right >= screenRect.Right && windowRect.Bottom >= screenRect.Bottom;
        }

        public virtual void Dispose()
        {
            UninstallHooks();
        }
    }
}