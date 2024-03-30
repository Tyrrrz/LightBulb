using System.Diagnostics;

namespace LightBulb.PlatformInterop;

public partial class DeviceContext(nint handle) : NativeResource(handle)
{
    private int _gammaChannelOffset;

    private void SetGammaRamp(GammaRamp ramp)
    {
        if (!NativeMethods.SetDeviceGammaRamp(Handle, ref ramp))
            Debug.WriteLine($"Failed to set gamma ramp on device context #{Handle}).");
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
            ramp.Red[i] = (ushort)(i * 255 * redMultiplier);
            ramp.Green[i] = (ushort)(i * 255 * greenMultiplier);
            ramp.Blue[i] = (ushort)(i * 255 * blueMultiplier);
        }

        // Some drivers will ignore requests to change gamma if the specified ramp is the same as last time,
        // even if the actual gamma has been changed in-between (for example, by screen going to sleep).
        // In order to work around this, we add a small random deviation to each ramp to make sure
        // they're always unique, forcing the drivers to refresh the device context every time.
        _gammaChannelOffset = ++_gammaChannelOffset % 5;
        ramp.Red[255] = (ushort)(ramp.Red[255] + _gammaChannelOffset);
        ramp.Green[255] = (ushort)(ramp.Green[255] + _gammaChannelOffset);
        ramp.Blue[255] = (ushort)(ramp.Blue[255] + _gammaChannelOffset);

        SetGammaRamp(ramp);
    }

    public void ResetGamma() => SetGamma(1, 1, 1);

    protected override void Dispose(bool disposing)
    {
        // Don't reset gamma during dispose because this method is also called whenever
        // the device context gets invalidated.
        // Resetting gamma in such cases will cause unwanted flickering.
        // https://github.com/Tyrrrz/LightBulb/issues/206

        if (!NativeMethods.DeleteDC(Handle))
            Debug.WriteLine($"Failed to dispose device context #{Handle}.");
    }
}

public partial class DeviceContext
{
    public static DeviceContext? TryGetByName(string deviceName)
    {
        var handle = NativeMethods.CreateDC(deviceName, null, null, 0);
        if (handle == 0)
        {
            Debug.WriteLine($"Failed to retrieve device context for '{deviceName}'.");
            return null;
        }

        return new DeviceContext(handle);
    }
}
