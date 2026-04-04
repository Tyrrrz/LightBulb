using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LightBulb.Core;

namespace LightBulb.PlatformInterop;

/// <summary>
/// Controls display gamma through the Windows GDI gamma-ramp API (via <see cref="DeviceContext"/>).
/// Supports both color temperature and brightness. Works on any display without additional software.
/// </summary>
public class WindowsNativeDisplayGammaController : IDisplayGammaController
{
    private IReadOnlyList<DeviceContext> _deviceContexts = [];
    private bool _areDeviceContextsValid;

    public string Id => "windows-native";
    public string DisplayName => "Windows (native gamma ramp)";
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

        foreach (var dc in _deviceContexts)
            dc.SetGamma(r, g, b);
    }

    public ValueTask ResetGammaAsync()
    {
        foreach (var dc in _deviceContexts)
            dc.ResetGamma();

        return ValueTask.CompletedTask;
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
