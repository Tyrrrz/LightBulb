using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using LightBulb.Models.WinApi;
using NegativeLayer.Extensions;

namespace LightBulb.Services
{
    public class WinApiService
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool SetDeviceGammaRamp(IntPtr hdc, ref Ramp lpRamp);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool GetDeviceGammaRamp(IntPtr hdc, ref Ramp lpRamp);

        private readonly Ramp _originalRamp;

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

        private void SetDisplayGammaRamp(Ramp ramp)
        {
            var dc = GetDC(IntPtr.Zero);
            if (!SetDeviceGammaRamp(dc, ref ramp))
                ThrowIfWin32Error();
        }

        private Ramp GetDisplayGammaRamp()
        {
            var dc = GetDC(IntPtr.Zero);
            var ramp = new Ramp();
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
            SetColorIntensity(1);
        }

        public void SetColorIntensity(double red, double green, double blue)
        {
            var ramp = new Ramp();
            ramp.Init();

            for (int i = 1; i < 256; i++)
            {
                ramp.Red[i] = (ushort) (i*255*red).RoundToInt().Clamp(ushort.MinValue, ushort.MaxValue);
                ramp.Green[i] = (ushort) (i*255*green).RoundToInt().Clamp(ushort.MinValue, ushort.MaxValue);
                ramp.Blue[i] = (ushort) (i*255*blue).RoundToInt().Clamp(ushort.MinValue, ushort.MaxValue);
            }

            SetDisplayGammaRamp(ramp);
        }

        public void SetColorIntensity(double uniform)
        {
            SetColorIntensity(uniform, uniform, uniform);
        }

        public void SetTemperature(ushort temp, double intensity = 1)
        {
            // http://www.tannerhelland.com/4435/convert-temperature-rgb-algorithm-code/

            double tempf = temp/100d;

            double redi;
            double greeni;
            double bluei;

            // Red
            if (tempf <= 66)
                redi = 1;
            else
                redi = (Math.Pow(tempf - 60, -0.1332047592)*329.698727446).Clamp(0, 255)/255d;

            // Green
            if (tempf <= 66)
                greeni = (Math.Log(tempf)*99.4708025861 - 161.1195681661).Clamp(0, 255)/255d;
            else
                greeni = (Math.Pow(tempf - 60, -0.0755148492)*288.1221695283).Clamp(0, 255)/255d;

            // Blue
            if (tempf >= 66)
                bluei = 1;
            else
            {
                if (tempf <= 19)
                    bluei = 0;
                else
                    bluei = (Math.Log(tempf - 10)*138.5177312231 - 305.0447927307).Clamp(0, 255)/255d;
            }

            redi *= intensity;
            greeni *= intensity;
            bluei *= intensity;

            SetColorIntensity(redi, greeni, bluei);
        }
    }
}