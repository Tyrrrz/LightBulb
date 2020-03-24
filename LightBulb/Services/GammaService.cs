using System;
using System.Collections.Generic;
using LightBulb.Models;
using LightBulb.WindowsApi;
using Microsoft.Win32;

namespace LightBulb.Services
{
    public class GammaService : IDisposable
    {
        private readonly object _lock = new object();

        // Device contexts for all monitors.
        // We can't just get device context for virtual screen because that doesn't work anymore since Win10 v1903.
        // Note: device contexts need to be refreshed if the user enables/disables monitors while the program is running.
        private IReadOnlyList<DeviceContext> _deviceContexts = Array.Empty<DeviceContext>();

        public GammaService()
        {
            SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;
            ResetDeviceContexts();
        }

        private void OnDisplaySettingsChanged(object? sender, EventArgs e) => ResetDeviceContexts();

        private void ResetDeviceContexts()
        {
            lock (_lock)
            {
                foreach (var deviceContext in _deviceContexts)
                    deviceContext.Dispose();

                _deviceContexts = DeviceContext.GetAllMonitorDeviceContexts();
            }
        }

        public void SetGamma(ColorConfiguration colorConfiguration)
        {
            // Algorithm taken from http://www.tannerhelland.com/4435/convert-temperature-rgb-algorithm-code/

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
            lock (_lock)
            {
                foreach (var deviceContext in _deviceContexts)
                {
                    deviceContext.SetGamma(
                        GetRed() * colorConfiguration.Brightness,
                        GetGreen() * colorConfiguration.Brightness,
                        GetBlue() * colorConfiguration.Brightness
                    );
                }
            }
        }

        public void Dispose()
        {
            lock (_lock)
            {
                foreach (var deviceContext in _deviceContexts)
                    deviceContext.Dispose();

                SystemEvents.DisplaySettingsChanged -= OnDisplaySettingsChanged;
            }
        }
    }
}