using System;
using System.Windows.Input;
using LightBulb.Models;
using LightBulb.WindowsApi;
using Microsoft.Win32;

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
            using var registry = OpenAutoStartRegistryKey(false);
            var value = registry?.GetValue(App.Name) as string;

            return string.Equals(value, AutoStartKeyValue, StringComparison.OrdinalIgnoreCase);
        }

        public void EnableAutoStart()
        {
            using var registry = OpenAutoStartRegistryKey(true);
            registry?.SetValue(App.Name, AutoStartKeyValue);
        }

        public void DisableAutoStart()
        {
            using var registry = OpenAutoStartRegistryKey(true);
            registry?.DeleteValue(App.Name, false);
        }

        public bool IsGammaRangeUnlocked()
        {
            using var registry = OpenGammaRangeRegistryKey(false);
            var value = registry?.GetValue("GdiICMGammaRange") as int?;

            return value == 256;
        }

        public void UnlockGammaRange()
        {
            using var registry = OpenGammaRangeRegistryKey(true);
            registry?.SetValue("GdiICMGammaRange", 256, RegistryValueKind.DWord);
        }

        public void Dispose()
        {
            _gammaManager.Dispose();
            _hotKeyManager.Dispose();
            _windowManager.Dispose();
        }
    }

    public partial class SystemService
    {
        private static string AutoStartKeyValue => $"\"{App.ExecutableFilePath}\" --autostart";

        private static RegistryKey OpenRegistryKey(RegistryKey parent, string subKey, bool write) =>
            write ? parent.CreateSubKey(subKey, true) : parent.OpenSubKey(subKey, false);

        private static RegistryKey OpenAutoStartRegistryKey(bool write) =>
            OpenRegistryKey(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Run", write);

        private static RegistryKey OpenGammaRangeRegistryKey(bool write) =>
            OpenRegistryKey(Registry.LocalMachine, "Software\\Microsoft\\Windows NT\\CurrentVersion\\ICM", write);
    }
}