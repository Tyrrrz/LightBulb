using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using LightBulb.Models;
using LightBulb.Models.WinApi;
using NegativeLayer.Extensions;

namespace LightBulb.Services
{
    public class WinApiService
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool SetDeviceGammaRamp(IntPtr hdc, ref GammaRamp lpRamp);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool GetDeviceGammaRamp(IntPtr hdc, ref GammaRamp lpRamp);

        private readonly GammaRamp _originalRamp;

        public WinApiService()
        {
            _originalRamp = GetDisplayGammaRamp();
        }

        private Win32Exception GetLastError()
        {
            int errCode = Marshal.GetLastWin32Error();
            if (errCode == 0) return null;
            return new Win32Exception(errCode);
        }

        private void ThrowIfWin32Error()
        {
            var ex = GetLastError();
            if (ex != null) throw ex;
        }

        public void SetDisplayGammaRamp(GammaRamp ramp)
        {
            var dc = GetDC(IntPtr.Zero);
            if (!SetDeviceGammaRamp(dc, ref ramp))
                ThrowIfWin32Error();
        }

        public GammaRamp GetDisplayGammaRamp()
        {
            var dc = GetDC(IntPtr.Zero);
            var ramp = new GammaRamp();
            if (!GetDeviceGammaRamp(dc, ref ramp))
                ThrowIfWin32Error();
            return ramp;
        }

        public void RestoreOriginal()
        {
            SetDisplayGammaRamp(_originalRamp);
        }

        public void RestoreDefault()
        {
            SetDisplayGammaLinear(new ColorIntensity(1));
        }

        public void SetDisplayGammaLinear(ColorIntensity intensity)
        {
            var ramp = new GammaRamp();
            ramp.Init();

            for (int i = 1; i < 256; i++)
            {
                ramp.Red[i] = (ushort) (i*255*intensity.Red).RoundToInt().Clamp(ushort.MinValue, ushort.MaxValue);
                ramp.Green[i] = (ushort) (i*255*intensity.Green).RoundToInt().Clamp(ushort.MinValue, ushort.MaxValue);
                ramp.Blue[i] = (ushort) (i*255*intensity.Blue).RoundToInt().Clamp(ushort.MinValue, ushort.MaxValue);
            }

            SetDisplayGammaRamp(ramp);
        }
    }
}