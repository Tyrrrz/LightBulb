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

        public IntPtr GetWindowProcessHandle(IntPtr hwnd)
        {
            NativeMethods.GetWindowThreadProcessId(hwnd, out var processId);
            return NativeMethods.OpenProcess(ProcessAccessFlags.QueryLimitedInformation, false, processId);
        }

        public string GetProcessExecutableFilePath(IntPtr hproc)
        {
            var fileNameBuilder = new StringBuilder(1024);
            var bufferLength = (uint)fileNameBuilder.Capacity + 1;
            return NativeMethods.QueryFullProcessImageName(hproc, 0, fileNameBuilder, ref bufferLength) ?
                fileNameBuilder.ToString() :
                "";
        }

        public bool IsWindowFullScreen(IntPtr hwnd)
        {
            // If window is a system window - return false
            var windowClassName = GetWindowClassName(hwnd);
            if (SystemWindowClassNames.Contains(windowClassName, StringComparer.OrdinalIgnoreCase))
                return false;

            // If window is not visible - return false;
            if (!NativeMethods.IsWindowVisible(hwnd))
                return false;

            // Get window rect
            var windowRect = GetWindowRect(hwnd);

            // If window rect is empty - return false
            if (windowRect == Rect.Empty)
                return false;

            // Get window client rect
            var windowClientRect = GetWindowClientRect(hwnd);

            // Calculate absolute window client rect (not relative to window rect)
            var absoluteWindowClientRect = new Rect(
                windowRect.Left + windowClientRect.Left,
                windowRect.Top + windowClientRect.Top,
                windowRect.Left + windowClientRect.Right,
                windowRect.Top + windowClientRect.Bottom
            );

            // Get screen rect
            var screenRect = Screen.FromHandle(hwnd).Bounds;

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