using System;
using Microsoft.Win32;

namespace LightBulb.WindowsApi.Utils.Extensions;

internal static class RegistryExtensions
{
    public static RegistryKey OpenKey(
        this RegistryHive hive,
        RegistryView view = RegistryView.Default
    ) => RegistryKey.OpenBaseKey(hive, view);

    public static string GetShortMoniker(this RegistryHive hive) =>
        hive switch
        {
            RegistryHive.ClassesRoot => "HKCR",
            RegistryHive.CurrentUser => "HKCU",
            RegistryHive.LocalMachine => "HKLM",
            RegistryHive.Users => "HKU",
            RegistryHive.PerformanceData => "HKPD",
            RegistryHive.CurrentConfig => "HKCC",
            _ => throw new ArgumentOutOfRangeException(nameof(hive))
        };
}
