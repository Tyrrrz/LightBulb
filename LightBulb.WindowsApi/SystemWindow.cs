using System;
using System.Text;
using System.Windows.Forms;
using LightBulb.WindowsApi.Internal;

namespace LightBulb.WindowsApi
{
    public partial class SystemWindow : IDisposable
    {
        public IntPtr Handle { get; }

        public SystemWindow(IntPtr handle)
        {
            Handle = handle;
        }

        private Rect GetRect()
        {
            NativeMethods.GetWindowRect(Handle, out var rect);
            return rect;
        }

        private Rect GetClientRect()
        {
            NativeMethods.GetClientRect(Handle, out var rect);
            return rect;
        }

        public string GetClassName()
        {
            var buffer = new StringBuilder(256);
            NativeMethods.GetClassName(Handle, buffer, buffer.Capacity);

            return buffer.ToString();
        }

        public bool IsSystemWindow()
        {
            var className = GetClassName();
            
            if (string.Equals(className, "Progman", StringComparison.OrdinalIgnoreCase))
                return true;

            if (string.Equals(className, "WorkerW", StringComparison.OrdinalIgnoreCase))
                return true;

            if (string.Equals(className, "ImmersiveLauncher", StringComparison.OrdinalIgnoreCase))
                return true;

            if (string.Equals(className, "ImmersiveSwitchList", StringComparison.OrdinalIgnoreCase))
                return true;

            if (string.Equals(className, "MultitaskingViewFrame", StringComparison.OrdinalIgnoreCase))
                return true;

            if (string.Equals(className, "ForegroundStaging", StringComparison.OrdinalIgnoreCase))
                return true;

            if (string.Equals(className, "ApplicationManager_DesktopShellWindow", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        public bool IsVisible() => NativeMethods.IsWindowVisible(Handle);

        public bool IsFullScreen()
        {
            // If window is a system window - return false
            if (IsSystemWindow())
                return false;

            // If window is not visible - return false
            if (!IsVisible())
                return false;

            // Get window rect
            var windowRect = GetRect();
            if (windowRect == Rect.Empty)
                return false;

            // Calculate absolute window client rect (not relative to window)
            var windowClientRect = GetClientRect();

            var absoluteWindowClientRect = new Rect(
                windowRect.Left + windowClientRect.Left,
                windowRect.Top + windowClientRect.Top,
                windowRect.Left + windowClientRect.Right,
                windowRect.Top + windowClientRect.Bottom
            );

            // Check if the window covers up screen bounds
            var screenRect = Screen.FromHandle(Handle).Bounds;

            return absoluteWindowClientRect.Left <= 0 &&
                   absoluteWindowClientRect.Top <= 0 &&
                   absoluteWindowClientRect.Right >= screenRect.Right &&
                   absoluteWindowClientRect.Bottom >= screenRect.Bottom;
        }

        public SystemProcess GetProcess()
        {
            NativeMethods.GetWindowThreadProcessId(Handle, out var processId);
            return SystemProcess.Open(processId);
        }

        public void Dispose()
        {
            // No cleaning up is needed, but this is for consistency
        }
    }

    public partial class SystemWindow
    {
        public static SystemWindow GetForegroundWindow()
        {
            var handle = NativeMethods.GetForegroundWindow();
            return new SystemWindow(handle);
        }
    }
}