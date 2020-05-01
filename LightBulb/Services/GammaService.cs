using System;
using System.Collections.Generic;
using LightBulb.Domain;
using LightBulb.Internal;
using LightBulb.WindowsApi;

namespace LightBulb.Services
{
    public class GammaService : IDisposable
    {
        // Device contexts for all monitors.
        // We can't just get device context for the virtual screen because that doesn't work anymore since Win10 v1903.
        // Wrapped in a dirty container to be able to invalidate the value when the device contexts change (e.g. a new monitor is plugged in).
        private readonly Dirty<IReadOnlyList<DeviceContext>> _deviceContextsDirty =
            Dirty.Create(DeviceContext.GetAllMonitorDeviceContexts, dcs => dcs.DisposeAll());

        private readonly SystemEventManager _systemEvents = new SystemEventManager();

        public GammaService()
        {
            _systemEvents.DisplaySettingsChanged += (sender, args) => _deviceContextsDirty.Invalidate();
        }

        public void SetGamma(ColorConfiguration colorConfiguration)
        {
            // Algorithm taken from http://tannerhelland.com/4435/convert-temperature-rgb-algorithm-code

            double GetRed()
            {
                if (colorConfiguration.Temperature > 6600)
                    return Math.Pow(colorConfiguration.Temperature / 100 - 60, -0.1332047592) * 329.698727446 / 255;

                return 1;
            }

            double GetGreen()
            {
                if (colorConfiguration.Temperature > 6600)
                    return Math.Pow(colorConfiguration.Temperature / 100 - 60, -0.0755148492) * 288.1221695283 / 255;

                return (Math.Log(colorConfiguration.Temperature / 100) * 99.4708025861 - 161.1195681661) / 255;
            }

            double GetBlue()
            {
                if (colorConfiguration.Temperature >= 6600)
                    return 1;

                if (colorConfiguration.Temperature <= 1900)
                    return 0;

                return (Math.Log(colorConfiguration.Temperature / 100 - 10) * 138.5177312231 - 305.0447927307) / 255;
            }

            // Set gamma
            foreach (var deviceContext in _deviceContextsDirty.Value)
            {
                deviceContext.SetGamma(
                    GetRed() * colorConfiguration.Brightness,
                    GetGreen() * colorConfiguration.Brightness,
                    GetBlue() * colorConfiguration.Brightness
                );
            }
        }

        public void Dispose()
        {
            _deviceContextsDirty.Dispose();
            _systemEvents.Dispose();
        }
    }
}