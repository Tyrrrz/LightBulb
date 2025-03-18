using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LightBulb.Core;
using LightBulb.PlatformInterop;
using LightBulb.Utils;
using LightBulb.Utils.Extensions;

namespace LightBulb.Services;

public partial class GammaService : IDisposable
{
    private readonly SettingsService _settingsService;
    private readonly DisposableCollector _eventRoot = new();

    private bool _isUpdatingGamma;

    private IReadOnlyList<DeviceContext> _deviceContexts = [];
    private bool _areDeviceContextsValid;
    private DateTimeOffset _lastGammaInvalidationTimestamp = DateTimeOffset.MinValue;

    private ColorConfiguration? _lastConfiguration;
    private DateTimeOffset _lastUpdateTimestamp = DateTimeOffset.MinValue;

    public GammaService(SettingsService settingsService)
    {
        _settingsService = settingsService;

        // Listen to all system events that may indicate that the device context or gamma was changed from the outside
        _eventRoot.Add(
            // https://github.com/Tyrrrz/LightBulb/issues/223
            SystemHook.TryRegister(SystemHook.Ids.ForegroundWindowChanged, InvalidateGamma)
                ?? Disposable.Null
        );

        _eventRoot.Add(
            PowerSettingNotification.TryRegister(
                PowerSettingNotification.Ids.ConsoleDisplayStateChanged,
                InvalidateGamma
            ) ?? Disposable.Null
        );

        _eventRoot.Add(
            PowerSettingNotification.TryRegister(
                PowerSettingNotification.Ids.PowerSavingStatusChanged,
                InvalidateGamma
            ) ?? Disposable.Null
        );

        _eventRoot.Add(
            PowerSettingNotification.TryRegister(
                PowerSettingNotification.Ids.SessionDisplayStatusChanged,
                InvalidateGamma
            ) ?? Disposable.Null
        );

        _eventRoot.Add(
            PowerSettingNotification.TryRegister(
                PowerSettingNotification.Ids.MonitorPowerStateChanged,
                InvalidateGamma
            ) ?? Disposable.Null
        );

        _eventRoot.Add(
            PowerSettingNotification.TryRegister(
                PowerSettingNotification.Ids.AwayModeChanged,
                InvalidateGamma
            ) ?? Disposable.Null
        );

        _eventRoot.Add(
            SystemEvent.Register(SystemEvent.Ids.DisplayChanged, InvalidateDeviceContexts)
        );

        _eventRoot.Add(
            SystemEvent.Register(SystemEvent.Ids.PaletteChanged, InvalidateDeviceContexts)
        );

        _eventRoot.Add(
            SystemEvent.Register(SystemEvent.Ids.SettingsChanged, InvalidateDeviceContexts)
        );

        _eventRoot.Add(
            SystemEvent.Register(SystemEvent.Ids.SystemColorsChanged, InvalidateDeviceContexts)
        );
    }

    private void EnsureValidDeviceContexts()
    {
        if (_areDeviceContextsValid)
            return;

        _areDeviceContextsValid = true;

        _deviceContexts.DisposeAll();
        _deviceContexts = Monitor
            .GetAll()
            .Select(m => m.TryCreateDeviceContext())
            .WhereNotNull()
            .ToArray();

        _lastConfiguration = null;
    }

    private bool IsGammaStale()
    {
        var instant = DateTimeOffset.Now;

        // Assume gamma continues to be stale for some time after it has been invalidated
        if ((instant - _lastGammaInvalidationTimestamp).Duration() <= TimeSpan.FromSeconds(0.3))
        {
            return true;
        }

        // If polling is enabled, assume gamma is stale after some time has passed since the last update
        if (
            _settingsService.IsGammaPollingEnabled
            && (instant - _lastUpdateTimestamp).Duration() > TimeSpan.FromSeconds(1)
        )
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

        return Math.Abs(configuration.Temperature - lastConfiguration.Temperature) > 15
            || Math.Abs(configuration.Brightness - lastConfiguration.Brightness) > 0.01;
    }

    public void InvalidateGamma()
    {
        // Don't invalidate gamma when we're in the process of changing it ourselves,
        // to avoid an infinite loop.
        if (_isUpdatingGamma)
            return;

        _lastGammaInvalidationTimestamp = DateTimeOffset.Now;
        Debug.WriteLine("Gamma invalidated.");
    }

    public void InvalidateDeviceContexts()
    {
        _areDeviceContextsValid = false;
        Debug.WriteLine("Device contexts invalidated.");

        InvalidateGamma();
    }

    public void SetGamma(ColorConfiguration configuration)
    {
        // Avoid unnecessary changes as updating too often will cause stuttering
        if (!IsGammaStale() && !IsSignificantChange(configuration))
            return;

        EnsureValidDeviceContexts();

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

        _eventRoot.Dispose();
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
                0,
                1
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
                Math.Pow(configuration.Temperature / 100 - 60, -0.0755148492)
                    * 288.1221695283
                    / 255,
                0,
                1
            );
        }

        return Math.Clamp(
            (Math.Log(configuration.Temperature / 100) * 99.4708025861 - 161.1195681661) / 255,
            0,
            1
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
            (Math.Log(configuration.Temperature / 100 - 10) * 138.5177312231 - 305.0447927307)
                / 255,
            0,
            1
        );
    }
}
