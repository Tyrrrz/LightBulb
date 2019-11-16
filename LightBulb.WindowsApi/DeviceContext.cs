using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using LightBulb.WindowsApi.Internal;

namespace LightBulb.WindowsApi
{
    public partial class DeviceContext : IDisposable
    {
        private int _gammaChannelOffset;

        public IntPtr Handle { get; }

        public DeviceContext(IntPtr handle)
        {
            Handle = handle;
        }

        ~DeviceContext()
        {
            Dispose();
        }

        private void SetGammaRamp(GammaRamp ramp) => NativeMethods.SetDeviceGammaRamp(Handle, ref ramp);

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
            for (var i = 0; i < 256; i++)
            {
                ramp.Red[i] = (ushort) (i * 255 * redMultiplier);
                ramp.Green[i] = (ushort) (i * 255 * greenMultiplier);
                ramp.Blue[i] = (ushort) (i * 255 * blueMultiplier);
            }

            // Some drivers will ignore request to change gamma if the ramp is the same as last time
            // so we randomize it a bit. Even though our ramp may not have changed, other applications
            // could have affected the gamma and we need to "force-refresh" it to ensure it's valid.
            _gammaChannelOffset = ++_gammaChannelOffset % 5;
            ramp.Red[255] = (ushort) (ramp.Red[255] + _gammaChannelOffset);
            ramp.Green[255] = (ushort) (ramp.Green[255] + _gammaChannelOffset);
            ramp.Blue[255] = (ushort) (ramp.Blue[255] + _gammaChannelOffset);

            // Set gamma
            SetGammaRamp(ramp);
        }

        public void Dispose()
        {
            SetGamma(1, 1, 1);
            NativeMethods.DeleteDC(Handle);
            GC.SuppressFinalize(this);
        }
    }

    public partial class DeviceContext
    {
        public static DeviceContext FromDeviceName(string deviceName) =>
            new DeviceContext(NativeMethods.CreateDC(deviceName, null, null, IntPtr.Zero));

        public static IReadOnlyList<DeviceContext> GetAllMonitorDeviceContexts() =>
            Screen.AllScreens.Select(s => FromDeviceName(s.DeviceName)).ToArray();
    }
}