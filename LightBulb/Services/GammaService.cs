using System;
using System.Collections.Generic;
using LightBulb.Models;
using LightBulb.WindowsApi;

namespace LightBulb.Services
{
    public class GammaService : IDisposable
    {
        // Device contexts for all monitors.
        // We can't just get device context for virtual screen because that doesn't work anymore since Win10 v1903.
        private readonly IReadOnlyList<NativeDeviceContext> _deviceContexts = NativeDeviceContext.GetAllMonitorDeviceContexts();

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
            foreach (var deviceContext in _deviceContexts)
                deviceContext.SetGamma(GetRed(), GetGreen(), GetBlue());
        }

        public void Dispose()
        {
            foreach (var deviceContext in _deviceContexts)
                deviceContext.Dispose();
        }
    }
}