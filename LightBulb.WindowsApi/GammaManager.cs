using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using LightBulb.WindowsApi.Internal;
using LightBulb.WindowsApi.Models;

namespace LightBulb.WindowsApi
{
    // This class doesn't use GetDC(IntPtr.Zero) to get DC for virtual screen because
    // that doesn't work on Win10 with multi-monitor setup since v1903.
    // https://github.com/Tyrrrz/LightBulb/issues/100
    public partial class GammaManager : IDisposable
    {
        private readonly object _lock = new object();
        private readonly IReadOnlyList<IntPtr> _deviceContextHandles;

        private int _gammaChannelOffset;

        public GammaManager(IReadOnlyList<IntPtr> deviceContextHandles)
        {
            _deviceContextHandles = deviceContextHandles;
        }

        public GammaManager() : this(GetDeviceContextHandlesForAllMonitors())
        {
        }

        ~GammaManager()
        {
            Dispose();
        }

        private GammaRamp CreateGammaRamp(ColorBalance colorBalance)
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

            return ramp;
        }

        public void SetGamma(ColorBalance colorBalance)
        {
            lock (_lock)
            {
                // Create ramp
                var ramp = CreateGammaRamp(colorBalance);

                // Set gamma
                foreach (var hdc in _deviceContextHandles)
                    NativeMethods.SetDeviceGammaRamp(hdc, ref ramp);
            }
        }

        public void Dispose()
        {
            foreach (var hdc in _deviceContextHandles)
                NativeMethods.DeleteDC(hdc);

            GC.SuppressFinalize(this);
        }
    }

    public partial class GammaManager
    {
        private static IReadOnlyList<IntPtr> GetDeviceContextHandlesForAllMonitors() =>
            Screen.AllScreens
                .Select(s => NativeMethods.CreateDC(s.DeviceName, null, null, IntPtr.Zero))
                .ToArray();
    }
}