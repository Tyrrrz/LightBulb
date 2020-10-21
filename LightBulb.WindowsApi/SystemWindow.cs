using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using LightBulb.WindowsApi.Internal;

namespace LightBulb.WindowsApi
{
    public partial class SystemWindow
    {
        public IntPtr Handle { get; }

        public SystemWindow(IntPtr handle) => Handle = handle;

        private Rect? TryGetRect() =>
            NativeMethods.GetWindowRect(Handle, out var rect)
                ? rect
                : (Rect?) null;

        private Rect? TryGetClientRect() =>
            NativeMethods.GetClientRect(Handle, out var rect)
                ? rect
                : (Rect?) null;

        public string? TryGetClassName()
        {
            var buffer = new StringBuilder(256);
            return NativeMethods.GetClassName(Handle, buffer, buffer.Capacity) >= 0
                ? buffer.ToString()
                : null;
        }

        public bool IsSystemWindow()
        {
            var className = TryGetClassName();

            return
                string.Equals(className, "Progman", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(className, "WorkerW", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(className, "ImmersiveLauncher", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(className, "ImmersiveSwitchList", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(className, "MultitaskingViewFrame", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(className, "ForegroundStaging", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(className, "ApplicationManager_DesktopShellWindow", StringComparison.OrdinalIgnoreCase);
        }

        public bool IsVisible() => NativeMethods.IsWindowVisible(Handle);

        public bool IsFullScreen()
        {
            // Get window rect
            var windowRect = TryGetRect() ?? Rect.Empty;
            if (windowRect == Rect.Empty)
                return false;

            // Calculate absolute window client rect (not relative to window)
            var windowClientRect = TryGetClientRect() ?? Rect.Empty;

            var absoluteWindowClientRect = new Rect(
                windowRect.Left + windowClientRect.Left,
                windowRect.Top + windowClientRect.Top,
                windowRect.Left + windowClientRect.Right,
                windowRect.Top + windowClientRect.Bottom
            );

            // Check if the window covers up screen bounds
            var screenRect = Screen.FromHandle(Handle).Bounds;

            return
                absoluteWindowClientRect.Left <= 0 &&
                absoluteWindowClientRect.Top <= 0 &&
                absoluteWindowClientRect.Right >= screenRect.Right &&
                absoluteWindowClientRect.Bottom >= screenRect.Bottom;
        }

        public SystemProcess? TryGetProcess()
        {
            NativeMethods.GetWindowThreadProcessId(Handle, out var processId);
            return processId != 0
                ? SystemProcess.TryOpen(processId)
                : null;
        }
    }

    public partial class SystemWindow
    {
        public static SystemWindow? TryGetForegroundWindow()
        {
            var handle = NativeMethods.GetForegroundWindow();
            return handle != IntPtr.Zero
                ? new SystemWindow(handle)
                : null;
        }

        public static IReadOnlyList<SystemWindow> GetAllWindows()
        {
            var result = new List<SystemWindow>();

            var callback = new NativeMethods.EnumWindowsProc((hWnd, lParam) =>
            {
                if (hWnd != IntPtr.Zero)
                {
                    var window = new SystemWindow(hWnd);
                    result.Add(window);
                }

                return true;
            });

            if (!NativeMethods.EnumWindows(callback, IntPtr.Zero))
                Debug.WriteLine("Failed to enumerate windows.");

            return result;
        }
    }
}