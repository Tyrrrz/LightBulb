using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace LightBulb.WindowsApi.Internal
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal static partial class NativeMethods
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(
            ProcessAccessFlags processAccess,
            bool bInheritHandle,
            uint processId
        );

        [DllImport("Kernel32.dll")]
        public static extern bool QueryFullProcessImageName([In] IntPtr hProcess, [In] uint dwFlags, [Out] StringBuilder lpExeName, [In, Out] ref uint lpdwSize);
    }

    internal static partial class NativeMethods
    {
        [DllImport("user32.dll", EntryPoint = "RegisterHotKey", SetLastError = true)]
        public static extern bool RegisterHotKey(IntPtr hwnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll", EntryPoint = "UnregisterHotKey", SetLastError = true)]
        public static extern bool UnregisterHotKey(IntPtr hwnd, int id);

        [DllImport("user32.dll", EntryPoint = "GetForegroundWindow", SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", EntryPoint = "GetWindowRect", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hwnd, out Rect lpRect);

        [DllImport("user32.dll", EntryPoint = "GetClientRect", SetLastError = true)]
        public static extern bool GetClientRect(IntPtr hwnd, out Rect lpRect);

        [DllImport("user32.dll", EntryPoint = "IsWindowVisible", SetLastError = true)]
        public static extern bool IsWindowVisible(IntPtr hwnd);

        [DllImport("user32.dll", EntryPoint = "GetClassName", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int GetClassName(IntPtr hwnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", EntryPoint = "GetWindowThreadProcessId", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint lpdwProcessId);
    }

    internal static partial class NativeMethods
    {
        [DllImport("gdi32.dll", EntryPoint = "CreateDC", SetLastError = true)]
        public static extern IntPtr CreateDC(string? lpszDriver, string? lpszDevice, string? lpszOutput, IntPtr lpInitData);

        [DllImport("gdi32.dll", EntryPoint = "DeleteDC", SetLastError = true)]
        public static extern bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", EntryPoint = "GetDeviceGammaRamp", SetLastError = true)]
        public static extern bool GetDeviceGammaRamp(IntPtr hdc, out GammaRamp lpRamp);

        [DllImport("gdi32.dll", EntryPoint = "SetDeviceGammaRamp", SetLastError = true)]
        public static extern bool SetDeviceGammaRamp(IntPtr hdc, ref GammaRamp lpRamp);
    }
}