using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using LightBulb.WindowsApi.Native;

namespace LightBulb.WindowsApi;

public partial class DeviceContext : IDisposable
{
    private readonly IntPtr _handle;

    private int _gammaChannelOffset;

    private DeviceContext(IntPtr handle) => _handle = handle;

    ~DeviceContext() => Dispose();

    private void SetGammaRamp(GammaRamp ramp)
    {
        if (!NativeMethods.SetDeviceGammaRamp(_handle, ref ramp))
            Debug.WriteLine($"Failed to set gamma ramp (handle: ${_handle}).");
    }

    public void SetGamma(double redMultiplier, double greenMultiplier, double blueMultiplier)
    {
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

        // Some drivers will ignore requests to change gamma if the specified ramp is the same as last time,
        // even if the actual gamma has been changed in-between (for example, by screen going to sleep).
        // In order to work around this, we add a small random deviation to each ramp to make sure
        // they're always unique, forcing the drivers to refresh the device context every time.
        _gammaChannelOffset = ++_gammaChannelOffset % 5;
        ramp.Red[255] = (ushort) (ramp.Red[255] + _gammaChannelOffset);
        ramp.Green[255] = (ushort) (ramp.Green[255] + _gammaChannelOffset);
        ramp.Blue[255] = (ushort) (ramp.Blue[255] + _gammaChannelOffset);

        SetGammaRamp(ramp);
    }

    public void ResetGamma() => SetGamma(1, 1, 1);

    public void Dispose()
    {
        // Don't reset gamma during dispose because this method
        // is also called whenever device context gets invalidated.
        // Resetting gamma in such cases will cause unwanted flickering.
        // https://github.com/Tyrrrz/LightBulb/issues/206

        if (!NativeMethods.DeleteDC(_handle))
            Debug.WriteLine($"Failed to dispose device context (handle: {_handle}).");

        GC.SuppressFinalize(this);
    }
}

public partial class DeviceContext
{
    public static DeviceContext? TryGetFromDeviceName(string deviceName)
    {
        var handle = NativeMethods.CreateDC(deviceName, null, null, IntPtr.Zero);

        if (handle == IntPtr.Zero)
        {
            Debug.WriteLine($"Failed to retrieve device context (device: '{deviceName}').");
            return null;
        }

        return new DeviceContext(handle);
    }

    public static IReadOnlyList<DeviceContext> FromAllMonitors()
    {
        var result = new List<DeviceContext>();

        foreach (var screen in Screen.AllScreens)
        {
            var deviceContext = TryGetFromDeviceName(screen.DeviceName);
            if (deviceContext is not null)
                result.Add(deviceContext);
        }

        return result;
    }
}