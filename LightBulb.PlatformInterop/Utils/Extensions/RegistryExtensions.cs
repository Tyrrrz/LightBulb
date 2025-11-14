using System;
using Microsoft.Win32;

namespace LightBulb.PlatformInterop.Utils.Extensions;

internal static class RegistryExtensions
{
    extension(RegistryHive hive)
    {
        public RegistryKey OpenKey(RegistryView view = RegistryView.Default) =>
            RegistryKey.OpenBaseKey(hive, view);

        public string GetShortMoniker() =>
            hive switch
            {
                RegistryHive.ClassesRoot => "HKCR",
                RegistryHive.CurrentUser => "HKCU",
                RegistryHive.LocalMachine => "HKLM",
                RegistryHive.Users => "HKU",
                RegistryHive.PerformanceData => "HKPD",
                RegistryHive.CurrentConfig => "HKCC",
                _ => throw new ArgumentOutOfRangeException(nameof(hive)),
            };
    }
}
