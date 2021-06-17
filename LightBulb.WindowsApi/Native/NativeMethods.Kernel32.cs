using System;
using System.Runtime.InteropServices;
using System.Text;

namespace LightBulb.WindowsApi.Native
{
    internal static partial class NativeMethods
    {
        private const string Kernel32 = "kernel32.dll";

        [DllImport(Kernel32, SetLastError = true)]
        public static extern IntPtr OpenProcess(
            ProcessAccessFlags processAccess,
            bool bInheritHandle,
            uint processId
        );

        [DllImport(Kernel32, SetLastError = true)]
        public static extern bool QueryFullProcessImageName(
            IntPtr hPrc,
            uint dwFlags,
            StringBuilder lpExeName,
            ref uint lpdwSize
        );

        [DllImport(Kernel32, SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObj);
    }
}