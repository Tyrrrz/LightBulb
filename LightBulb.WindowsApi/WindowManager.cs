using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LightBulb.WindowsApi.Internal;

namespace LightBulb.WindowsApi
{
    public partial class WindowManager : IDisposable
    {
        private readonly WinEvent _foregroundWindowChangedEvent;
        private WinEvent _foregroundWindowLocationChangedEvent;

        private IntPtr _lastForegroundWindow;

        public event EventHandler ForegroundWindowChanged;
        public event EventHandler ForegroundWindowLocationChanged; 

        public WindowManager()
        {
            var foregroundWindowLocationChangedEventHandler = new NativeMethods.WinEventHandler(
                (hook, type, hWnd, idObject, child, thread, time) =>
                {
                    // Only events from windows
                    if (idObject != 0)
                        return;

                    // Only events from foreground window
                    if (hWnd != _lastForegroundWindow)
                        return;

                    // Fire event
                    ForegroundWindowLocationChanged?.Invoke(this, EventArgs.Empty);
                });

            var foregroundWindowChangedEventHandler = new NativeMethods.WinEventHandler(
                (hook, type, hWnd, idObject, child, thread, time) =>
                {
                    // Only events from windows
                    if (idObject != 0)
                        return;

                    // Remember foreground window
                    _lastForegroundWindow = hWnd;

                    // Unhook location change event
                    _foregroundWindowLocationChangedEvent?.Dispose();

                    // Hook location change event of foreground window
                    _foregroundWindowLocationChangedEvent =
                        WinEvent.Register(0x800B, thread, foregroundWindowLocationChangedEventHandler);

                    // Fire event
                    ForegroundWindowChanged?.Invoke(this, EventArgs.Empty);
                });

            // Register foreground window change event
            _foregroundWindowChangedEvent = WinEvent.Register(0x0003, foregroundWindowChangedEventHandler);
        }

        private string GetWindowClassName(IntPtr hWnd)
        {
            var buffer = new StringBuilder(256);
            NativeMethods.GetClassName(hWnd, buffer, buffer.Capacity);

            return buffer.ToString();
        }

        private Rect GetWindowRect(IntPtr hWnd)
        {
            NativeMethods.GetWindowRect(hWnd, out var rect);
            return rect;
        }

        private Rect GetWindowClientRect(IntPtr hWnd)
        {
            NativeMethods.GetClientRect(hWnd, out var rect);
            return rect;
        }

        private bool IsWindowFullScreen(IntPtr hWnd)
        {
            // If window is a system window - return false
            var windowClassName = GetWindowClassName(hWnd);
            if (SystemWindowClassNames.Contains(windowClassName, StringComparer.OrdinalIgnoreCase))
                return false;

            // If window is not visible - return false;
            if (!NativeMethods.IsWindowVisible(hWnd))
                return false;

            // Get window rect
            var windowRect = GetWindowRect(hWnd);

            // If window rect is empty - return false
            if (windowRect == Rect.Empty)
                return false;

            // Get client rect
            var clientRect = GetWindowClientRect(hWnd);

            // Calculate absolute client rect (not relative to window rect)
            var absoluteClientRect = new Rect(
                windowRect.Left + clientRect.Left,
                windowRect.Top + clientRect.Top,
                windowRect.Left + clientRect.Right,
                windowRect.Top + clientRect.Bottom
            );

            // Get screen rect
            var screenRect = Screen.FromHandle(hWnd).Bounds;

            // Bounding check
            return absoluteClientRect.Left <= 0 && absoluteClientRect.Top <= 0 &&
                   absoluteClientRect.Right >= screenRect.Right && absoluteClientRect.Bottom >= screenRect.Bottom;
        }

        public bool IsForegroundWindowFullScreen()
        {
            var hWnd = NativeMethods.GetForegroundWindow();

            // If failed for some reason - return false
            if (hWnd == IntPtr.Zero)
                return false;

            return IsWindowFullScreen(hWnd);
        }

        public void Dispose()
        {
            _foregroundWindowChangedEvent.Dispose();
            _foregroundWindowLocationChangedEvent?.Dispose();
        }
    }

    public partial class WindowManager
    {
        private static readonly string[] SystemWindowClassNames =
        {
            "Progman", "WorkerW", "ImmersiveLauncher", "ImmersiveSwitchList", "MultitaskingViewFrame",
            "ForegroundStaging", "ApplicationManager_DesktopShellWindow"
        };
    }
}