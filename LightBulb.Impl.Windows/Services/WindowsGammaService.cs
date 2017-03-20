using System;
using LightBulb.Internal;
using LightBulb.Models;

namespace LightBulb.Services
{
    public class WindowsGammaService : WinApiServiceBase, IGammaService
    {
        private readonly IntPtr _dc;
        private int _gammaChannelOffset;

        public WindowsGammaService()
        {
            _dc = NativeMethods.GetDCInternal(IntPtr.Zero);
            CheckLogWin32Error();
        }

        /// <summary>
        /// Get the curve that represents the current display gamma
        /// </summary>
        public GammaRamp GetDisplayGammaRamp()
        {
            GammaRamp ramp;
            if (!NativeMethods.GetDeviceGammaRampInternal(_dc, out ramp))
                CheckLogWin32Error();
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
            _gammaChannelOffset = _gammaChannelOffset++%5;
            ramp.Red[255] = (ushort) (ramp.Red[255] + _gammaChannelOffset);
            ramp.Green[255] = (ushort) (ramp.Green[255] + _gammaChannelOffset);
            ramp.Blue[255] = (ushort) (ramp.Blue[255] + _gammaChannelOffset);

            // Set ramp
            if (!NativeMethods.SetDeviceGammaRampInternal(_dc, ref ramp))
                CheckLogWin32Error();
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
            NativeMethods.ReleaseDCInternal(IntPtr.Zero, _dc);
            CheckLogWin32Error();
            base.Dispose(disposing);
        }
    }
}