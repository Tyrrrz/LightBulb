using System;
using System.Text;
using System.Windows.Forms;
using LightBulb.Internal;
using LightBulb.Models;

namespace LightBulb.Services
{
    public class WindowsWindowService : IWindowService, IDisposable
    {
        private readonly HookManager _hookManager;

        private IntPtr _foregroundWindowLocationChangedHook;

        private IntPtr _lastForegroundWindow;
        private bool _isForegroundFullScreen;

        /// <inheritdoc />
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

        /// <inheritdoc />
        public event EventHandler FullScreenStateChanged;

        public WindowsWindowService()
        {
            _hookManager = new HookManager();

            var foregroundWindowLocationChangedEventHandler = new WinEventHandler(
                (hook, type, hwnd, idObject, child, thread, time) =>
                {
                    if (idObject != 0) return; // only events from windows
                    if (hwnd != _lastForegroundWindow) return; // skip non-foreground windows

                    IsForegroundFullScreen = IsWindowFullScreen(hwnd);
                });
            var foregroundWindowChangedEventHandler = new WinEventHandler(
                (hook, type, hwnd, idObject, child, thread, time) =>
                {
                    if (idObject != 0) return; // only events from windows

                    _lastForegroundWindow = hwnd;
                    IsForegroundFullScreen = IsWindowFullScreen(hwnd);

                    // Hook location changed event for foreground window
                    if (_foregroundWindowLocationChangedHook != IntPtr.Zero)
                        _hookManager.UnregisterWinEvent(_foregroundWindowLocationChangedHook);
                    _foregroundWindowLocationChangedHook = _hookManager.RegisterWinEvent(0x800B,
                        foregroundWindowLocationChangedEventHandler, 0, thread);
                });

            _hookManager.RegisterWinEvent(0x0003, foregroundWindowChangedEventHandler);

            // Init
            IsForegroundFullScreen = IsWindowFullScreen(GetForegroundWindow());
        }

        ~WindowsWindowService()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the window handle for the current foreground window
        /// </summary>
        public IntPtr GetForegroundWindow()
        {
            var result = NativeMethods.GetForegroundWindow();
            return result;
        }

        /// <summary>
        /// Gets the window handle for the desktop window
        /// </summary>
        public IntPtr GetDesktopWindow()
        {
            var result = NativeMethods.GetDesktopWindow();
            return result;
        }

        /// <summary>
        /// Gets the window handle for the shell window
        /// </summary>
        public IntPtr GetShellWindow()
        {
            var result = NativeMethods.GetShellWindow();
            return result;
        }

        /// <summary>
        /// Gets the rectangle bounds of the given window
        /// </summary>
        public Rect GetWindowRect(IntPtr hWindow)
        {
            Rect result;
            NativeMethods.GetWindowRect(hWindow, out result);
            return result;
        }

        /// <summary>
        /// Gets the client rectangle bounds of the given window
        /// </summary>
        public Rect GetWindowClientRect(IntPtr hWindow)
        {
            Rect result;
            NativeMethods.GetWindowClientRect(hWindow, out result);
            return result;
        }

        /// <summary>
        /// Gets whether the window is visible
        /// </summary>
        public bool IsWindowVisible(IntPtr hWindow)
        {
            bool result = NativeMethods.IsWindowVisible(hWindow);
            return result;
        }

        /// <summary>
        /// Gets the window class name
        /// </summary>
        public string GetClassName(IntPtr hWindow)
        {
            var sb = new StringBuilder(256);
            NativeMethods.GetClassName(hWindow, sb, sb.Capacity);
            return sb.ToString();
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

            // If window is desktop or shell - return
            if (hWindow == desktop || hWindow == shell)
                return false;

            // If window is wallpaper - return
            string className = GetClassName(hWindow);
            if (className.Equals("Progman", StringComparison.OrdinalIgnoreCase) ||
                className.Equals("WorkerW", StringComparison.OrdinalIgnoreCase))
                return false;

            // If not visible - return
            if (!IsWindowVisible(hWindow))
                return false;

            // Get the window rect
            var windowRect = GetWindowRect(hWindow);

            // If window doesn't have a rect - return
            if (windowRect.Left <= 0 && windowRect.Top <= 0 && windowRect.Right <= 0 && windowRect.Bottom <= 0)
                return false;

            // Get client rect and actual rect
            var clientRect = GetWindowClientRect(hWindow);
            var actualRect = new Rect(
                windowRect.Left + clientRect.Left,
                windowRect.Top + clientRect.Top,
                windowRect.Left + clientRect.Right,
                windowRect.Top + clientRect.Bottom
            );

            // Get the screen rect and do a bounding box check
            var screenRect = Screen.FromHandle(hWindow).Bounds;
            bool boundCheck = actualRect.Left <= 0 && actualRect.Top <= 0 &&
                              actualRect.Right >= screenRect.Right && actualRect.Bottom >= screenRect.Bottom;

            return boundCheck;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _hookManager.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}