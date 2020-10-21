using System;
using System.Collections.Generic;
using LightBulb.Domain;
using LightBulb.Internal;
using LightBulb.Internal.Extensions;
using LightBulb.WindowsApi;

namespace LightBulb.Services
{
    public partial class GammaService : IDisposable
    {
        private readonly IDisposable _eventRegistration;

        private readonly SettingsService _settingsService;

        private IReadOnlyList<DeviceContext> _deviceContexts = Array.Empty<DeviceContext>();
        private bool _isDeviceContextsValid;
        private bool _isGammaValid;

        private ColorConfiguration? _lastConfiguration;
        private DateTimeOffset _lastUpdateTimestamp = DateTimeOffset.MinValue;

        public GammaService(SettingsService settingsService)
        {
            _settingsService = settingsService;

            // Register for all system events that may indicate that device context has changed
            _eventRegistration = Disposable.Aggregate(
                PowerSettingNotification.TryRegister(PowerSettingNotification.ConsoleDisplayStateId,
                    InvalidateGamma) ?? Disposable.Null,
                PowerSettingNotification.TryRegister(PowerSettingNotification.PowerSavingStatusId,
                    InvalidateGamma) ?? Disposable.Null,
                PowerSettingNotification.TryRegister(PowerSettingNotification.SessionDisplayStatusId,
                    InvalidateGamma) ?? Disposable.Null,
                PowerSettingNotification.TryRegister(PowerSettingNotification.MonitorPowerOnId,
                    InvalidateGamma) ?? Disposable.Null,
                PowerSettingNotification.TryRegister(PowerSettingNotification.AwayModeId,
                    InvalidateGamma) ?? Disposable.Null,
                SystemEvent.Register(SystemEvent.DisplayChangedId, InvalidateDeviceContext),
                SystemEvent.Register(SystemEvent.PaletteChangedId, InvalidateDeviceContext),
                SystemEvent.Register(SystemEvent.SettingsChangedId, InvalidateDeviceContext),
                SystemEvent.Register(SystemEvent.SystemColorsChangedId, InvalidateDeviceContext)
            );
        }

        private void InvalidateDeviceContext() => _isDeviceContextsValid = false;

        private void InvalidateGamma() => _isGammaValid = false;

        private void EnsureValidDeviceContext()
        {
            if (_isDeviceContextsValid)
                return;

            _isDeviceContextsValid = true;

            _deviceContexts.DisposeAll();
            _deviceContexts = DeviceContext.FromAllMonitors();

            _lastConfiguration = null;
        }

        private bool IsSignificantChange(ColorConfiguration configuration) =>
            _lastConfiguration == null ||
            Math.Abs(configuration.Temperature - _lastConfiguration.Value.Temperature) > 25 ||
            Math.Abs(configuration.Brightness - _lastConfiguration.Value.Brightness) > 0.01;

        private bool IsGammaStale() =>
            !_isGammaValid ||
            _settingsService.IsGammaPollingEnabled &&
            (DateTimeOffset.Now - _lastUpdateTimestamp).Duration() > TimeSpan.FromSeconds(1);

        public void SetGamma(ColorConfiguration configuration)
        {
            // Avoid unnecessary changes as updating too often will cause stutters
            if (!IsGammaStale() && !IsSignificantChange(configuration))
                return;

            EnsureValidDeviceContext();

            _deviceContexts.ForEach(i => i.SetGamma(
                GetRed(configuration) * configuration.Brightness,
                GetGreen(configuration) * configuration.Brightness,
                GetBlue(configuration) * configuration.Brightness
            ));

            _isGammaValid = true;
            _lastConfiguration = configuration;
            _lastUpdateTimestamp = DateTimeOffset.Now;
        }

        public void Dispose()
        {
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
                return Math.Pow(configuration.Temperature / 100 - 60, -0.1332047592) * 329.698727446 / 255;

            return 1;
        }

        private static double GetGreen(ColorConfiguration configuration)
        {
            // Algorithm taken from http://tannerhelland.com/4435/convert-temperature-rgb-algorithm-code

            if (configuration.Temperature > 6600)
                return Math.Pow(configuration.Temperature / 100 - 60, -0.0755148492) * 288.1221695283 / 255;

            return (Math.Log(configuration.Temperature / 100) * 99.4708025861 - 161.1195681661) / 255;
        }

        private static double GetBlue(ColorConfiguration configuration)
        {
            // Algorithm taken from http://tannerhelland.com/4435/convert-temperature-rgb-algorithm-code

            if (configuration.Temperature >= 6600)
                return 1;

            if (configuration.Temperature <= 1900)
                return 0;

            return (Math.Log(configuration.Temperature / 100 - 10) * 138.5177312231 - 305.0447927307) / 255;
        }
    }
}