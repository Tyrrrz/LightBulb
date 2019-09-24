using System;
using System.Windows.Input;
using LightBulb.Models;
using LightBulb.WindowsApi;
using LightBulb.WindowsApi.Models;

namespace LightBulb.Services
{
    public class SystemService : IDisposable
    {
        private readonly GammaManager _gammaManager = new GammaManager();
        private readonly HotKeyManager _hotKeyManager = new HotKeyManager();
        private readonly WindowManager _windowManager = new WindowManager();

        private static ColorBalance GetColorBalance(ColorTemperature temperature)
        {
            // Algorithm taken from http://www.tannerhelland.com/4435/convert-temperature-rgb-algorithm-code/

            double GetRed()
            {
                if (temperature.Value > 6600)
                    return Math.Pow(temperature.Value / 100 - 60, -0.1332047592) * 329.698727446 / 255;

                return 1;
            }

            double GetGreen()
            {
                if (temperature.Value > 6600)
                    return Math.Pow(temperature.Value / 100 - 60, -0.0755148492) * 288.1221695283 / 255;

                return (Math.Log(temperature.Value / 100) * 99.4708025861 - 161.1195681661) / 255;
            }

            double GetBlue()
            {
                if (temperature.Value >= 6600)
                    return 1;

                if (temperature.Value <= 1900)
                    return 0;

                return (Math.Log(temperature.Value / 100 - 10) * 138.5177312231 - 305.0447927307) / 255;
            }

            return new ColorBalance(GetRed(), GetGreen(), GetBlue());
        }

        public void SetGamma(ColorTemperature temperature) => _gammaManager.SetGamma(GetColorBalance(temperature));

        public void RegisterHotKey(HotKey hotKey, Action handler)
        {
            var virtualKey = KeyInterop.VirtualKeyFromKey(hotKey.Key);
            var modifiers = (int) hotKey.Modifiers;

            _hotKeyManager.RegisterHotKey(virtualKey, modifiers, handler);
        }

        public void UnregisterAllHotKeys() => _hotKeyManager.UnregisterAllHotKeys();

        public bool IsForegroundWindowFullScreen()
        {
            var foregroundWindow = _windowManager.GetForegroundWindow();
            return _windowManager.IsWindowFullScreen(foregroundWindow);
        }

        public void Dispose()
        {
            _gammaManager.Dispose();
            _hotKeyManager.Dispose();
            _windowManager.Dispose();
        }
    }
}