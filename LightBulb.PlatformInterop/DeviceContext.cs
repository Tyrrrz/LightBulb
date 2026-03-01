using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using LightBulb.PlatformInterop.Internal;

namespace LightBulb.PlatformInterop;

public partial class DeviceContext(nint handle, IReadOnlyList<PhysicalMonitor> physicalMonitors)
    : NativeResource(handle)
{
    private readonly IReadOnlyList<PhysicalMonitor> _physicalMonitors = physicalMonitors;
    private int _gammaChannelOffset;

    private void SetGammaRamp(GammaRamp ramp)
    {
        if (!NativeMethods.SetDeviceGammaRamp(Handle, ref ramp))
        {
            Debug.WriteLine(
                $"Failed to set gamma ramp on device context #{Handle}). "
                    + $"Error {Marshal.GetLastWin32Error()}."
            );
        }
    }

    public void SetGamma(double redMultiplier, double greenMultiplier, double blueMultiplier)
    {
        var ramp = new GammaRamp
        {
            Red = new ushort[256],
            Green = new ushort[256],
            Blue = new ushort[256],
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

    public void SetBrightness(double brightness)
    {
        // Brightness is a value from 0.0 to 1.0, convert to 0-100
        var brightnessPercentage = (uint)Math.Clamp(brightness * 100, 0, 100);

        foreach (var physicalMonitor in _physicalMonitors)
        {
            if (!NativeMethods.SetMonitorBrightness(physicalMonitor.Handle, brightnessPercentage))
            {
                Debug.WriteLine(
                    $"Failed to set brightness on monitor #{physicalMonitor.Handle}. "
                        + $"Error {Marshal.GetLastWin32Error()}."
                );
            }
        }
    }

    public void ResetGamma() => SetGamma(1, 1, 1);

    protected override void Dispose(bool disposing)
    {
        if (_physicalMonitors.Any())
        {
            // This is a bit of a hack, but we need to convert the read-only list to an array
            var physicalMonitorsArray = _physicalMonitors.ToArray();

            if (
                !NativeMethods.DestroyPhysicalMonitors(
                    (uint)physicalMonitorsArray.Length,
                    physicalMonitorsArray
                )
            )
            {
                Debug.WriteLine(
                    "Failed to destroy physical monitors. "
                        + $"Error {Marshal.GetLastWin32Error()}."
                );
            }
        }

        // Don't reset gamma during dispose because this method is also called whenever
        // the device context gets invalidated.
        // Resetting gamma in such cases will cause unwanted flickering.
        // https://github.com/Tyrrrz/LightBulb/issues/206
        if (!NativeMethods.DeleteDC(Handle))
        {
            Debug.WriteLine(
                $"Failed to dispose device context #{Handle}. "
                    + $"Error {Marshal.GetLastWin32Error()}."
            );
        }
    }
}

public partial class DeviceContext
{
    public static DeviceContext? TryCreate(
        string deviceName,
        IReadOnlyList<PhysicalMonitor> physicalMonitors
    )
    {
        var handle = NativeMethods.CreateDC(deviceName, deviceName, null, 0);
        if (handle == 0)
        {
            Debug.WriteLine(
                $"Failed to retrieve device context for '{deviceName}'. "
                    + $"Error {Marshal.GetLastWin32Error()}."
            );
            return null;
        }

        return new DeviceContext(handle, physicalMonitors);
    }
}
