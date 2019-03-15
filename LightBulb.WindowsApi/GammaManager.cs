using System;
using LightBulb.WindowsApi.Internal;
using LightBulb.WindowsApi.Models;
using Microsoft.Win32;

namespace LightBulb.WindowsApi
{
    public class GammaManager : IDisposable
    {
        private readonly IntPtr _window;
        private readonly IntPtr _deviceContext;

        private ColorBalance _lastColorBalance = ColorBalance.Default;
        private int _gammaChannelOffset;

        public GammaManager(IntPtr window)
        {
            _window = window;
            _deviceContext = NativeMethods.GetDC(window);

            // Refresh gamma on specific system events
            SystemEvents.DisplaySettingsChanging += (sender, args) => SetGamma(_lastColorBalance);
            SystemEvents.PowerModeChanged += (sender, args) => SetGamma(_lastColorBalance);
        }

        public GammaManager() : this(IntPtr.Zero)
        {
        }

        ~GammaManager()
        {
            Dispose();
        }

        public void SetGamma(ColorBalance colorBalance)
        {
            // Create native ramp object
            var ramp = new GammaRamp
            {
                Red = new ushort[256],
                Green = new ushort[256],
                Blue = new ushort[256]
            };

            // Create linear ramps for each color
            for (var i = 0; i < 256; i++)
            {
                ramp.Red[i] = (ushort) (i * 255 * colorBalance.Red);
                ramp.Green[i] = (ushort) (i * 255 * colorBalance.Green);
                ramp.Blue[i] = (ushort) (i * 255 * colorBalance.Blue);
            }

            // Some drivers will ignore request to change gamma if the ramp is the same as last time
            // so we randomize it a bit. Even though our ramp may not have changed, other applications
            // could have affected the gamma and we need to "force-refresh" it to ensure it's valid.
            _gammaChannelOffset = ++_gammaChannelOffset % 5;
            ramp.Red[255] = (ushort) (ramp.Red[255] + _gammaChannelOffset);
            ramp.Green[255] = (ushort) (ramp.Green[255] + _gammaChannelOffset);
            ramp.Blue[255] = (ushort) (ramp.Blue[255] + _gammaChannelOffset);

            // Set gamma
            NativeMethods.SetDeviceGammaRamp(_deviceContext, ref ramp);
            _lastColorBalance = colorBalance;
        }

        public void Dispose()
        {
            NativeMethods.ReleaseDC(_window, _deviceContext);
            GC.SuppressFinalize(this);
        }
    }
}