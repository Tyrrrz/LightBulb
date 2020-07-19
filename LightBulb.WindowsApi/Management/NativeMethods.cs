using System;
using System.Runtime.InteropServices;
using System.Text;

namespace LightBulb.WindowsApi.Management
{
    internal static class NativeMethods
    {
        private const string Kernel = "kernel32.dll";
        private const string User = "user32.dll";

        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport(Kernel, SetLastError = true)]
        public static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, uint processId);

        [DllImport(Kernel, SetLastError = true)]
        public static extern bool QueryFullProcessImageName(IntPtr hPrc, uint dwFlags, StringBuilder lpExeName, ref uint lpdwSize);

        [DllImport(Kernel, SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObj);

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
    }
}