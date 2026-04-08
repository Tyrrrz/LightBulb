using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LightBulb.Core;

namespace LightBulb.PlatformInterop;

/// <summary>
/// Controls display gamma on Linux via xrandr (works on X11 and XWayland sessions).
/// Supports both color temperature and brightness.
/// </summary>
public class LinuxXorgDisplayGammaController : IDisplayColorController
{
    private IReadOnlyList<DeviceContext> _deviceContexts = [];
    private bool _areDeviceContextsValid;

    public string Id => "linux-xorg";
    public bool IsBrightnessSupported => true;

    private async Task EnsureDeviceContextsValidAsync()
    {
        if (_areDeviceContextsValid)
            return;

        _areDeviceContextsValid = true;

        foreach (var dc in _deviceContexts)
            dc.Dispose();

        _deviceContexts = (await Monitor.GetAllAsync())
            .Select(m => m.TryCreateDeviceContext())
            .Where(dc => dc is not null)
            .Select(dc => dc!)
            .ToArray();
    }

    public async ValueTask SetGammaAsync(ColorConfiguration configuration)
    {
        await EnsureDeviceContextsValidAsync();

        var r =
            ColorTemperature.GetRedMultiplier(configuration.Temperature) * configuration.Brightness;
        var g =
            ColorTemperature.GetGreenMultiplier(configuration.Temperature)
            * configuration.Brightness;
        var b =
            ColorTemperature.GetBlueMultiplier(configuration.Temperature)
            * configuration.Brightness;

        await Task.WhenAll(_deviceContexts.Select(dc => dc.SetGammaAsync(r, g, b).AsTask()));
    }

    public async ValueTask ResetGammaAsync()
    {
        await Task.WhenAll(_deviceContexts.Select(dc => dc.ResetGammaAsync().AsTask()));
    }

    public void Invalidate()
    {
        _areDeviceContextsValid = false;
        Debug.WriteLine($"[{Id}] Display configuration changed — device contexts invalidated.");
    }

    public void Dispose()
    {
        foreach (var dc in _deviceContexts)
            dc.Dispose();
    }
}
