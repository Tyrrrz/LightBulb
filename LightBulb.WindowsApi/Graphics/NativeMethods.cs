using System;
using System.Runtime.InteropServices;

namespace LightBulb.WindowsApi.Graphics
{
    internal static class NativeMethods
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