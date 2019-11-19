using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace LightBulb.WindowsApi.Internal
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal static partial class NativeMethods
    {
        private const string Kernel = "kernel32.dll";

        [DllImport(Kernel, SetLastError = true)]
        public static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, uint processId);

        [DllImport(Kernel, SetLastError = true)]
        public static extern bool QueryFullProcessImageName(IntPtr hPrc, uint dwFlags, StringBuilder lpExeName, ref uint lpdwSize);

        [DllImport(Kernel, SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObj);
    }

    internal static partial class NativeMethods
    {
        private const string User = "user32.dll";

        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport(User, SetLastError = true)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport(User, SetLastError = true)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport(User, SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport(User, SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);

        [DllImport(User, SetLastError = true)]
        public static extern bool GetClientRect(IntPtr hWnd, out Rect lpRect);

        [DllImport(User, SetLastError = true)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport(User, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport(User, SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport(User, SetLastError = true)]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport(User, SetLastError = true)]
        public static extern IntPtr RegisterPowerSettingNotification(IntPtr hRecipient, ref Guid powerSettingGuid, Int32 flags);

        [DllImport(User, SetLastError = true)]
        public static extern bool UnregisterPowerSettingNotification(in IntPtr handle);
    }

    internal static partial class NativeMethods
    {
        private const string Gdi = "gdi32.dll";

        [DllImport(Gdi, SetLastError = true)]
        public static extern IntPtr CreateDC(string? lpszDriver, string? lpszDevice, string? lpszOutput, IntPtr lpInitData);

        [DllImport(Gdi, SetLastError = true)]
        public static extern bool DeleteDC(IntPtr hDc);

        [DllImport(Gdi, SetLastError = true)]
        public static extern bool GetDeviceGammaRamp(IntPtr hDc, out GammaRamp lpRamp);

        [DllImport(Gdi, SetLastError = true)]
        public static extern bool SetDeviceGammaRamp(IntPtr hDc, ref GammaRamp lpRamp);
    }
}