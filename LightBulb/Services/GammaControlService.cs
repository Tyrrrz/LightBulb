using System;
using System.Runtime.InteropServices;
using LightBulb.Models;
using NegativeLayer.Extensions;

namespace LightBulb.Services
{
    public class GammaControlService : WinApiServiceBase
    {
        [DllImport("user32.dll", EntryPoint = "GetDC", SetLastError = true)]
        private static extern IntPtr GetDCInternal(IntPtr hWnd);

        [DllImport("gdi32.dll", EntryPoint = "SetDeviceGammaRamp", SetLastError = true)]
        private static extern bool SetDeviceGammaRampInternal(IntPtr hMonitor, ref GammaRamp ramp);

        [DllImport("gdi32.dll", EntryPoint = "GetDeviceGammaRamp", SetLastError = true)]
        private static extern bool GetDeviceGammaRampInternal(IntPtr hMonitor, out GammaRamp ramp);

        private readonly GammaRamp _originalRamp;

        public GammaControlService()
        {
            _originalRamp = GetDisplayGammaRamp();
        }

        /// <summary>
        /// Get the curve that represents the current display gamma
        /// </summary>
        public GammaRamp GetDisplayGammaRamp()
        {
            var dc = GetDCInternal(IntPtr.Zero);
            GammaRamp ramp;
            if (!GetDeviceGammaRampInternal(dc, out ramp))
                CheckThrowWin32Error();
            return ramp;
        }

        /// <summary>
        /// Change the display gamma based on given curve
        /// </summary>
        public void SetDisplayGammaRamp(GammaRamp ramp)
        {
            // Randomize the values in ramp slightly...
            // ... this forces the ramp to refresh every time
            // ... because some drivers will just reject it if it doesn't change
            ramp.Red[255] = (ushort) (ramp.Red[255] + Ext.RandomInt(-5, 5));
            ramp.Blue[255] = (ushort) (ramp.Blue[255] + Ext.RandomInt(-5, 5));
            ramp.Green[255] = (ushort) (ramp.Green[255] + Ext.RandomInt(-5, 5));

            // Set ramp
            var dc = GetDCInternal(IntPtr.Zero);
            if (!SetDeviceGammaRampInternal(dc, ref ramp))
                CheckThrowWin32Error();
        }

        /// <summary>
        /// Change the display gamma by multiplying each channel with a scalar
        /// </summary>
        public void SetDisplayGammaLinear(ColorIntensity intensity)
        {
            var ramp = new GammaRamp();
            ramp.Init();

            for (int i = 1; i < 256; i++)
            {
                ramp.Red[i] = (ushort) (i*255*intensity.Red).RoundToInt();
                ramp.Green[i] = (ushort) (i*255*intensity.Green).RoundToInt();
                ramp.Blue[i] = (ushort) (i*255*intensity.Blue).RoundToInt();
            }

            SetDisplayGammaRamp(ramp);
        }

        /// <summary>
        /// Restore gamma that was used before initializing the service
        /// </summary>
        public void RestoreOriginal()
        {
            SetDisplayGammaRamp(_originalRamp);
        }

        /// <summary>
        /// Restore the default gamma
        /// </summary>
        public void RestoreDefault()
        {
            SetDisplayGammaLinear(new ColorIntensity(1));
        }
    }
}