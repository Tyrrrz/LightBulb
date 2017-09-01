using System;
using LightBulb.Internal;
using LightBulb.Models;

namespace LightBulb.Services
{
    public class WindowsGammaService : IGammaService, IDisposable
    {
        private readonly IntPtr _dc;
        private int _gammaChannelOffset;

        public WindowsGammaService()
        {
            _dc = NativeMethods.GetDC(IntPtr.Zero);
        }

        ~WindowsGammaService()
        {
            Dispose(false);
        }

        /// <summary>
        /// Get the curve that represents the current display gamma
        /// </summary>
        public GammaRamp GetDisplayGammaRamp()
        {
            GammaRamp ramp;
            NativeMethods.GetDeviceGammaRamp(_dc, out ramp);
            return ramp;
        }

        /// <summary>
        /// Change the display gamma based on given curve
        /// </summary>
        private void SetDisplayGammaRamp(GammaRamp ramp)
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
            NativeMethods.SetDeviceGammaRamp(_dc, ref ramp);
        }

        /// <inheritdoc />
        public void SetDisplayGammaLinear(ColorIntensity intensity)
        {
            var ramp = new GammaRamp(256);

            for (var i = 1; i < 256; i++)
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

        protected void Dispose(bool disposing)
        {
            RestoreDefault();
            NativeMethods.ReleaseDC(IntPtr.Zero, _dc);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}