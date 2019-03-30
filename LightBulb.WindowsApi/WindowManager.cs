using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LightBulb.WindowsApi.Internal;

namespace LightBulb.WindowsApi
{
    public partial class WindowManager : IDisposable
    {
        public IntPtr GetForegroundWindow() => NativeMethods.GetForegroundWindow();

        public bool IsWindowFullScreen(IntPtr hWnd)
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

            // Get window client rect
            var windowClientRect = GetWindowClientRect(hWnd);

            // Calculate absolute window client rect (not relative to window rect)
            var absoluteWindowClientRect = new Rect(
                windowRect.Left + windowClientRect.Left,
                windowRect.Top + windowClientRect.Top,
                windowRect.Left + windowClientRect.Right,
                windowRect.Top + windowClientRect.Bottom
            );

            // Get screen rect
            var screenRect = Screen.FromHandle(hWnd).Bounds;

            // Bounding box check
            return absoluteWindowClientRect.Left <= 0 &&
                   absoluteWindowClientRect.Top <= 0 &&
                   absoluteWindowClientRect.Right >= screenRect.Right &&
                   absoluteWindowClientRect.Bottom >= screenRect.Bottom;
        }

        public void Dispose()
        {
            // Disposable for consistency with other WinApi manager classes
        }
    }

    public partial class WindowManager
    {
        private static readonly string[] SystemWindowClassNames =
        {
            "Progman", "WorkerW", "ImmersiveLauncher", "ImmersiveSwitchList", "MultitaskingViewFrame",
            "ForegroundStaging", "ApplicationManager_DesktopShellWindow"
        };

        private static string GetWindowClassName(IntPtr hWnd)
        {
            var buffer = new StringBuilder(256);
            NativeMethods.GetClassName(hWnd, buffer, buffer.Capacity);

            return buffer.ToString();
        }

        private static Rect GetWindowRect(IntPtr hWnd)
        {
            NativeMethods.GetWindowRect(hWnd, out var rect);
            return rect;
        }

        private static Rect GetWindowClientRect(IntPtr hWnd)
        {
            NativeMethods.GetClientRect(hWnd, out var rect);
            return rect;
        }
    }
}