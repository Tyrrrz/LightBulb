using System;
using System.Runtime.InteropServices;

namespace LightBulb.WindowsApi.Native
{
    internal static partial class NativeMethods
    {
        private const string Gdi32 = "gdi32.dll";

        [DllImport(Gdi32, SetLastError = true)]
        public static extern IntPtr CreateDC(
            string? lpszDriver,
            string? lpszDevice,
            string? lpszOutput,
            IntPtr lpInitData
        );

        [DllImport(Gdi32, SetLastError = true)]
        public static extern bool DeleteDC(IntPtr hDc);

        [DllImport(Gdi32, SetLastError = true)]
        public static extern bool GetDeviceGammaRamp(IntPtr hDc, out GammaRamp lpRamp);

        [DllImport(Gdi32, SetLastError = true)]
        public static extern bool SetDeviceGammaRamp(IntPtr hDc, ref GammaRamp lpRamp);
    }
}