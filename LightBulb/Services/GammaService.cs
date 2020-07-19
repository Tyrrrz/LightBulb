using System;
using LightBulb.Domain;
using LightBulb.Internal;
using LightBulb.WindowsApi.Events;
using LightBulb.WindowsApi.Graphics;

namespace LightBulb.Services
{
    public partial class GammaService : IDisposable
    {
        private readonly SettingsService _settingsService;

        private readonly IDisposable _systemEventRegistration;

        private IDeviceContext _deviceContext = DeviceContext.GetVirtualMonitor();
        private bool _isDeviceContextValid = true;

        private ColorConfiguration? _lastColorConfiguration;
        private DateTimeOffset _lastUpdateTimestamp = DateTimeOffset.MinValue;

        public GammaService(SettingsService settingsService)
        {
            _settingsService = settingsService;

            // If display settings change, invalidate the device context
            _systemEventRegistration = Disposable.Aggregate(
                SystemEvent.TryRegister(SystemEventType.DisplaySettingsChanged, () => _isDeviceContextValid = false),
                SystemEvent.TryRegister(SystemEventType.DisplayStateChanged, () => _isDeviceContextValid = false)
            );
        }

        private void EnsureDeviceContextIsValid()
        {
            if (_isDeviceContextValid)
                return;

            _isDeviceContextValid = true;

            _deviceContext.Dispose();
            _deviceContext = DeviceContext.GetVirtualMonitor();

            _lastColorConfiguration = null;
        }

        // We want to avoid insignificant changes to gamma as it causes stutters
        private bool IsSignificantChange(ColorConfiguration colorConfiguration) =>
            _lastColorConfiguration == null ||
            Math.Abs(colorConfiguration.Temperature - _lastColorConfiguration.Value.Temperature) > 25 ||
            Math.Abs(colorConfiguration.Brightness - _lastColorConfiguration.Value.Brightness) > 0.01;

        // If enabled in settings, gamma becomes stale after X seconds, forcing a refresh regardless of the color configuration changes
        private bool IsGammaStale() =>
            _settingsService.IsGammaPollingEnabled &&
            (DateTimeOffset.Now - _lastUpdateTimestamp).Duration() > TimeSpan.FromSeconds(1);

        public void SetGamma(ColorConfiguration colorConfiguration)
        {
            // Skip if gamma doesn't need refreshing and color configuration didn't change much since last time
            if (!IsGammaStale() && !IsSignificantChange(colorConfiguration))
                return;

            EnsureDeviceContextIsValid();

            _deviceContext.SetGamma(
                GetRed(colorConfiguration) * colorConfiguration.Brightness,
                GetGreen(colorConfiguration) * colorConfiguration.Brightness,
                GetBlue(colorConfiguration) * colorConfiguration.Brightness
            );

            _lastColorConfiguration = colorConfiguration;
            _lastUpdateTimestamp = DateTimeOffset.Now;
        }

        public void Dispose()
        {
            _deviceContext.Dispose();
            _systemEventRegistration.Dispose();
        }
    }

    public partial class GammaService
    {
        private static double GetRed(ColorConfiguration colorConfiguration)
        {
            // Algorithm taken from http://tannerhelland.com/4435/convert-temperature-rgb-algorithm-code

            if (colorConfiguration.Temperature > 6600)
                return Math.Pow(colorConfiguration.Temperature / 100 - 60, -0.1332047592) * 329.698727446 / 255;

            return 1;
        }

        private static double GetGreen(ColorConfiguration colorConfiguration)
        {
            // Algorithm taken from http://tannerhelland.com/4435/convert-temperature-rgb-algorithm-code

            if (colorConfiguration.Temperature > 6600)
                return Math.Pow(colorConfiguration.Temperature / 100 - 60, -0.0755148492) * 288.1221695283 / 255;

            return (Math.Log(colorConfiguration.Temperature / 100) * 99.4708025861 - 161.1195681661) / 255;
        }

        private static double GetBlue(ColorConfiguration colorConfiguration)
        {
            // Algorithm taken from http://tannerhelland.com/4435/convert-temperature-rgb-algorithm-code

            if (colorConfiguration.Temperature >= 6600)
                return 1;

            if (colorConfiguration.Temperature <= 1900)
                return 0;

            return (Math.Log(colorConfiguration.Temperature / 100 - 10) * 138.5177312231 - 305.0447927307) / 255;
        }
    }
}