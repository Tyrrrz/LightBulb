using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Disposables;
using LightBulb.Core;
using LightBulb.Utils.Extensions;
using LightBulb.WindowsApi;

namespace LightBulb.Services;

public partial class GammaService : IDisposable
{
    private readonly SettingsService _settingsService;
    private readonly IDisposable _eventRegistration;

    private bool _isUpdatingGamma;

    private IReadOnlyList<DeviceContext> _deviceContexts = Array.Empty<DeviceContext>();
    private bool _isValidDeviceContextHandle;
    private DateTimeOffset _lastGammaInvalidationTimestamp = DateTimeOffset.MinValue;

    private ColorConfiguration? _lastConfiguration;
    private DateTimeOffset _lastUpdateTimestamp = DateTimeOffset.MinValue;

    public GammaService(SettingsService settingsService)
    {
        _settingsService = settingsService;

        // Register for all system events that may indicate that device context or gamma was changed from outside
        _eventRegistration = new[]
        {
            // https://github.com/Tyrrrz/LightBulb/issues/223
            SystemHook.TryRegister(
                SystemHook.ForegroundWindowChanged,
                InvalidateGamma
            ) ?? Disposable.Empty,

            PowerSettingNotification.TryRegister(
                PowerSettingNotification.Ids.ConsoleDisplayStateChanged,
                InvalidateGamma
            ) ?? Disposable.Empty,
            PowerSettingNotification.TryRegister(
                PowerSettingNotification.Ids.PowerSavingStatusChanged,
                InvalidateGamma
            ) ?? Disposable.Empty,
            PowerSettingNotification.TryRegister(
                PowerSettingNotification.Ids.SessionDisplayStatusChanged,
                InvalidateGamma
            ) ?? Disposable.Empty,
            PowerSettingNotification.TryRegister(
                PowerSettingNotification.Ids.MonitorPowerStateChanged,
                InvalidateGamma
            ) ?? Disposable.Empty,
            PowerSettingNotification.TryRegister(
                PowerSettingNotification.Ids.AwayModeChanged,
                InvalidateGamma
            ) ?? Disposable.Empty,

            SystemEvent.Register(
                SystemEvent.Ids.DisplayChanged,
                InvalidateDeviceContext
            ),
            SystemEvent.Register(
                SystemEvent.Ids.PaletteChanged,
                InvalidateDeviceContext
            ),
            SystemEvent.Register(
                SystemEvent.Ids.SettingsChanged,
                InvalidateDeviceContext
            ),
            SystemEvent.Register(
                SystemEvent.Ids.SystemColorsChanged,
                InvalidateDeviceContext
            )
        }.Aggregate();
    }

    private void InvalidateGamma()
    {
        // Don't invalidate gamma when we're in the process of changing it ourselves,
        // to avoid an infinite loop.
        if (_isUpdatingGamma)
            return;

        _lastGammaInvalidationTimestamp = DateTimeOffset.Now;
        Debug.WriteLine("Gamma invalidated.");
    }

    private void InvalidateDeviceContext()
    {
        _isValidDeviceContextHandle = false;
        Debug.WriteLine("Device context invalidated.");

        InvalidateGamma();
    }

    private void EnsureValidDeviceContext()
    {
        if (_isValidDeviceContextHandle)
            return;

        _isValidDeviceContextHandle = true;

        _deviceContexts.DisposeAll();
        _deviceContexts = DeviceContext.FromAllMonitors();

        _lastConfiguration = null;
    }

    private bool IsGammaStale()
    {
        var instant = DateTimeOffset.Now;

        // Assume gamma continues to be stale for some time after it has been invalidated
        if ((instant - _lastGammaInvalidationTimestamp).Duration() <= TimeSpan.FromSeconds(0.5))
        {
            return true;
        }

        // If polling is enabled, assume gamma is stale after some time has passed since last update
        if (_settingsService.IsGammaPollingEnabled &&
            (instant - _lastUpdateTimestamp).Duration() > TimeSpan.FromSeconds(1))
        {
            return true;
        }

        return false;
    }

    private bool IsSignificantChange(ColorConfiguration configuration)
    {
        // Nothing to compare to
        if (_lastConfiguration is not { } lastConfiguration)
            return true;

        return
            Math.Abs(configuration.Temperature - lastConfiguration.Temperature) > 15 ||
            Math.Abs(configuration.Brightness - lastConfiguration.Brightness) > 0.01;
    }

    public void SetGamma(ColorConfiguration configuration)
    {
        // Avoid unnecessary changes as updating too often will cause stutters
        if (!IsGammaStale() && !IsSignificantChange(configuration))
            return;

        EnsureValidDeviceContext();

        _isUpdatingGamma = true;

        foreach (var deviceContext in _deviceContexts)
        {
            deviceContext.SetGamma(
                GetRed(configuration) * configuration.Brightness,
                GetGreen(configuration) * configuration.Brightness,
                GetBlue(configuration) * configuration.Brightness
            );
        }

        _isUpdatingGamma = false;

        _lastConfiguration = configuration;
        _lastUpdateTimestamp = DateTimeOffset.Now;
        Debug.WriteLine($"Updated gamma to {configuration}.");
    }

    public void Dispose()
    {
        // Reset gamma on all contexts
        foreach (var deviceContext in _deviceContexts)
            deviceContext.ResetGamma();

        _eventRegistration.Dispose();
        _deviceContexts.DisposeAll();
    }
}

public partial class GammaService
{
    private static double GetRed(ColorConfiguration configuration)
    {
        // Algorithm taken from http://tannerhelland.com/4435/convert-temperature-rgb-algorithm-code

        if (configuration.Temperature > 6600)
        {
            return Math.Clamp(
                Math.Pow(configuration.Temperature / 100 - 60, -0.1332047592) * 329.698727446 / 255,
                0, 1
            );
        }

        return 1;
    }

    private static double GetGreen(ColorConfiguration configuration)
    {
        // Algorithm taken from http://tannerhelland.com/4435/convert-temperature-rgb-algorithm-code

        if (configuration.Temperature > 6600)
        {
            return Math.Clamp(
                Math.Pow(configuration.Temperature / 100 - 60, -0.0755148492) * 288.1221695283 / 255,
                0, 1
            );
        }

        return Math.Clamp(
            (Math.Log(configuration.Temperature / 100) * 99.4708025861 - 161.1195681661) / 255,
            0, 1
        );
    }

    private static double GetBlue(ColorConfiguration configuration)
    {
        // Algorithm taken from http://tannerhelland.com/4435/convert-temperature-rgb-algorithm-code

        if (configuration.Temperature >= 6600)
            return 1;

        if (configuration.Temperature <= 1900)
            return 0;

        return Math.Clamp(
            (Math.Log(configuration.Temperature / 100 - 10) * 138.5177312231 - 305.0447927307) / 255,
            0, 1
        );
    }
}