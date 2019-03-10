using System;
using LightBulb.WindowsApi.Internal;

namespace LightBulb.WindowsApi
{
    public class GammaManager : IDisposable
    {
        private readonly IntPtr _window;
        private readonly IntPtr _deviceContext;

        private int _gammaChannelOffset;

        public GammaManager(IntPtr window)
        {
            _window = window;
            _deviceContext = NativeMethods.GetDC(window);
        }

        public GammaManager() : this(IntPtr.Zero)
        {
        }

        ~GammaManager()
        {
            Dispose();
        }

        public void SetGamma(double redMultiplier, double greenMultiplier, double blueMultiplier)
        {
            // Create native ramp object
            var ramp = new GammaRamp
            {
                Red = new ushort[256],
                Green = new ushort[256],
                Blue = new ushort[256]
            };

            // Create linear ramps for each color
            for (var i = 1; i < 256; i++)
            {
                ramp.Red[i] = (ushort) (i * 255 * redMultiplier);
                ramp.Green[i] = (ushort) (i * 255 * greenMultiplier);
                ramp.Blue[i] = (ushort) (i * 255 * blueMultiplier);
            }

            // Some drivers will ignore request to change gamma if the ramp is the same as last time
            // so we randomize it a bit
            _gammaChannelOffset = ++_gammaChannelOffset % 5;
            ramp.Red[255] = (ushort) (ramp.Red[255] + _gammaChannelOffset);
            ramp.Green[255] = (ushort) (ramp.Green[255] + _gammaChannelOffset);
            ramp.Blue[255] = (ushort) (ramp.Blue[255] + _gammaChannelOffset);

            // Set gamma
            NativeMethods.SetDeviceGammaRamp(_deviceContext, ref ramp);
        }

        public void Dispose()
        {
            // Release DC
            NativeMethods.ReleaseDC(_window, _deviceContext);
        }
    }
}