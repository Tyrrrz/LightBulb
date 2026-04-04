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
public class LinuxXorgDisplayGammaController : IDisplayGammaController
{
    private IReadOnlyList<DeviceContext> _deviceContexts = [];
    private bool _areDeviceContextsValid;

    public string Id => "linux-xorg";
    public string DisplayName => "X.Org / XWayland (xrandr)";
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
            ColorTemperatureConversion.GetRedMultiplier(configuration.Temperature)
            * configuration.Brightness;
        var g =
            ColorTemperatureConversion.GetGreenMultiplier(configuration.Temperature)
            * configuration.Brightness;
        var b =
            ColorTemperatureConversion.GetBlueMultiplier(configuration.Temperature)
            * configuration.Brightness;

        await Task.WhenAll(_deviceContexts.Select(dc => dc.SetGammaAsync(r, g, b).AsTask()));
    }

    public async ValueTask ResetGammaAsync()
    {
        await Task.WhenAll(_deviceContexts.Select(dc => dc.ResetGammaAsync().AsTask()));
    }

    public void NotifyDisplayConfigurationChanged()
    {
        _areDeviceContextsValid = false;
        Debug.WriteLine(
            $"[{DisplayName}] Display configuration changed — device contexts invalidated."
        );
    }

    public void Dispose()
    {
        foreach (var dc in _deviceContexts)
            dc.Dispose();
    }
}
