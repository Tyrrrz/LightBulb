using System;
using System.Runtime.InteropServices;
using LightBulb.Models;
using LightBulb.Services.Abstract;
using LightBulb.Services.Interfaces;

namespace LightBulb.Services
{
    public class WindowsGammaService : WinApiServiceBase, IGammaService
    {
        #region WinAPI

        [DllImport("user32.dll", EntryPoint = "GetDC", SetLastError = true)]
        private static extern IntPtr GetDCInternal(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "ReleaseDC", SetLastError = true)]
        private static extern int ReleaseDCInternal(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", EntryPoint = "SetDeviceGammaRamp", SetLastError = true)]
        private static extern bool SetDeviceGammaRampInternal(IntPtr hMonitor, ref GammaRamp ramp);

        [DllImport("gdi32.dll", EntryPoint = "GetDeviceGammaRamp", SetLastError = true)]
        private static extern bool GetDeviceGammaRampInternal(IntPtr hMonitor, out GammaRamp ramp);

        #endregion

        private static readonly object Lock = new object();

        private int _gammaChannelOffset;

        ~WindowsGammaService()
        {
            Dispose(false);
        }

        /// <summary>
        /// Get the curve that represents the current display gamma
        /// </summary>
        public GammaRamp GetDisplayGammaRamp()
        {
            var dc = GetDCInternal(IntPtr.Zero);
            GammaRamp ramp;
            if (!GetDeviceGammaRampInternal(dc, out ramp))
                CheckLogWin32Error();
            ReleaseDCInternal(IntPtr.Zero, dc);
            return ramp;
        }

        /// <summary>
        /// Change the display gamma based on given curve
        /// </summary>
        public void SetDisplayGammaRamp(GammaRamp ramp)
        {
            lock (Lock)
            {
                // Offset the values in ramp slightly...
                // ... this forces the ramp to refresh every time
                // ... because some drivers will ignore stale ramps
                // ... while the gamma itself might have been changed
                _gammaChannelOffset = ++_gammaChannelOffset%5;
                ramp.Red[255] = (ushort) (ramp.Red[255] + _gammaChannelOffset);
                ramp.Green[255] = (ushort) (ramp.Green[255] + _gammaChannelOffset);
                ramp.Blue[255] = (ushort) (ramp.Blue[255] + _gammaChannelOffset);

                // Set ramp
                var dc = GetDCInternal(IntPtr.Zero);
                if (!SetDeviceGammaRampInternal(dc, ref ramp))
                    CheckLogWin32Error();
                ReleaseDCInternal(IntPtr.Zero, dc);
            }
        }

        /// <inheritdoc />
        public void SetDisplayGammaLinear(ColorIntensity intensity)
        {
            var ramp = new GammaRamp(256);

            for (int i = 1; i < 256; i++)
            {
                ramp.Red[i] = (ushort) (i*255*intensity.Red);
                ramp.Green[i] = (ushort) (i*255*intensity.Green);
                ramp.Blue[i] = (ushort) (i*255*intensity.Blue);
            }

            SetDisplayGammaRamp(ramp);
        }

        /// <inheritdoc />
        public void RestoreDefault()
        {
            SetDisplayGammaLinear(ColorIntensity.Identity);
        }

        protected override void Dispose(bool disposing)
        {
            RestoreDefault();
            base.Dispose(disposing);
        }
    }
}