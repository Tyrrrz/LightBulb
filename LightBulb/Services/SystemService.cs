using System;
using System.Windows.Input;
using LightBulb.Internal;
using LightBulb.Models;
using LightBulb.WindowsApi;

namespace LightBulb.Services
{
    public partial class SystemService : IDisposable
    {
        private readonly GammaManager _gammaManager = new GammaManager();
        private readonly HotKeyManager _hotKeyManager = new HotKeyManager();
        private readonly WindowManager _windowManager = new WindowManager();

        public void SetGamma(ColorConfiguration colorConfiguration) => _gammaManager.SetGamma(colorConfiguration.ToColorBalance());

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

        public bool IsAutoStartEnabled()
        {
            var registryKeyEntryValue = RegistryEx.GetValueOrDefault<string>(AutoStartRegistryPath, AutoStartRegistryEntryName);
            return string.Equals(registryKeyEntryValue, AutoStartRegistryEntryValue, StringComparison.OrdinalIgnoreCase);
        }

        public void EnableAutoStart() =>
            RegistryEx.SetValue(AutoStartRegistryPath, AutoStartRegistryEntryName, AutoStartRegistryEntryValue);

        public void DisableAutoStart() =>
            RegistryEx.DeleteValue(AutoStartRegistryPath, AutoStartRegistryEntryName);

        public bool IsGammaRangeUnlocked() =>
            RegistryEx.GetValueOrDefault<int>(GammaRangeRegistryPath, GammaRangeRegistryEntryName) == GammaRangeRegistryEntryValue;

        public void UnlockGammaRange() =>
            RegistryEx.SetValue(GammaRangeRegistryPath, GammaRangeRegistryEntryName, GammaRangeRegistryEntryValue);

        public void Dispose()
        {
            _gammaManager.Dispose();
            _hotKeyManager.Dispose();
            _windowManager.Dispose();
        }
    }

    public partial class SystemService
    {
        private const string AutoStartRegistryPath = "HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Run";

        private static string AutoStartRegistryEntryName => App.Name;

        private static string AutoStartRegistryEntryValue => $"\"{App.ExecutableFilePath}\" --autostart";

        private const string GammaRangeRegistryPath = "HKLM\\Software\\Microsoft\\Windows NT\\CurrentVersion\\ICM";

        private const string GammaRangeRegistryEntryName = "GdiICMGammaRange";

        private const int GammaRangeRegistryEntryValue = 256;
    }
}