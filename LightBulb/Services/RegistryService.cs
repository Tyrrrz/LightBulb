using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security;
using LightBulb.Internal;
using Microsoft.Win32;
using Tyrrrz.Extensions;

namespace LightBulb.Services
{
    public partial class RegistryService
    {
        public bool IsAutoStartEnabled()
        {
            var registryKeyEntryValue = GetValueOrDefault<string>(AutoStartRegistryPath, AutoStartRegistryEntryName);
            return string.Equals(registryKeyEntryValue, AutoStartRegistryEntryValue, StringComparison.OrdinalIgnoreCase);
        }

        public void EnableAutoStart() =>
            SetValue(AutoStartRegistryPath, AutoStartRegistryEntryName, AutoStartRegistryEntryValue);

        public void DisableAutoStart() =>
            DeleteValue(AutoStartRegistryPath, AutoStartRegistryEntryName);

        public bool IsGammaRangeUnlocked() =>
            GetValueOrDefault<int>(GammaRangeRegistryPath, GammaRangeRegistryEntryName) == GammaRangeRegistryEntryValue;

        public void UnlockGammaRange() =>
            SetValue(GammaRangeRegistryPath, GammaRangeRegistryEntryName, GammaRangeRegistryEntryValue);
    }

    public partial class RegistryService
    {
        private const string AutoStartRegistryPath = "HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Run";

        private static string AutoStartRegistryEntryName => App.Name;

        private static string AutoStartRegistryEntryValue => $"\"{App.ExecutableFilePath}\" {App.HiddenOnLaunchArgument}";

        private const string GammaRangeRegistryPath = "HKLM\\Software\\Microsoft\\Windows NT\\CurrentVersion\\ICM";

        private const string GammaRangeRegistryEntryName = "GdiICMGammaRange";

        private const int GammaRangeRegistryEntryValue = 256;
    }

    public partial class RegistryService
    {
        private static RegistryKey GetRegistryKeyFromHiveName(string hiveName)
        {
            if (string.Equals(hiveName, "HKLM", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(hiveName, "HKEY_LOCAL_MACHINE", StringComparison.OrdinalIgnoreCase))
            {
                return Registry.LocalMachine;
            }

            if (string.Equals(hiveName, "HKCU", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(hiveName, "HKEY_CURRENT_USER", StringComparison.OrdinalIgnoreCase))
            {
                return Registry.CurrentUser;
            }

            // Others are not supported because they're not used in this application
            throw new NotSupportedException($"Unsupported or invalid hive [{hiveName}].");
        }

        private static string GetRegistryValueType(Type type)
        {
            if (type == typeof(int))
                return "REG_DWORD";

            if (type == typeof(string))
                return "REG_SZ";

            // Others are not supported because they're not used in this application
            throw new NotSupportedException($"Unsupported registry value type [{type}].");
        }

        private static void RunCommandLine(IReadOnlyList<string> arguments, bool asAdmin = false)
        {
            var processInfo = new ProcessStartInfo("reg");

            processInfo.ArgumentList.AddRange(arguments);

            if (asAdmin)
            {
                processInfo.UseShellExecute = true;
                processInfo.Verb = "runas";
            }

            using (Process.Start(processInfo))
            {
            }

            // Warn: this doesn't wait for exit and doesn't ensure success
        }

        private static RegistryKey GetRegistryKey(string path, bool needsWriteAccess)
        {
            var hiveName = path.SubstringUntil("\\");
            var relativePath = path.SubstringAfter("\\");

            var parentKey = GetRegistryKeyFromHiveName(hiveName);

            return needsWriteAccess
                ? parentKey.CreateSubKey(relativePath, true)
                : parentKey.OpenSubKey(relativePath, false);
        }

        public static T GetValueOrDefault<T>(string path, string entryName, T defaultValue = default)
        {
            // No command line fallback because this one should never have security issues, hopefully
            using var registryKey = GetRegistryKey(path, false);
            return registryKey?.GetValue(entryName) is T convertedValue ? convertedValue : defaultValue;
        }

        public static void SetValue(string path, string entryName, object entryValue)
        {
            try
            {
                using var registryKey = GetRegistryKey(path, true);
                registryKey.SetValue(entryName, entryValue);
            }
            catch (Exception e) when (e is SecurityException || e is UnauthorizedAccessException)
            {
                RunCommandLine(new[]
                {
                    "add", path,
                    "/v", entryName,
                    "/d", entryValue.ToString()!,
                    "/t", GetRegistryValueType(entryValue.GetType()),
                    "/f"
                }, true);
            }
        }

        public static void DeleteValue(string path, string entryName)
        {
            try
            {
                using var registryKey = GetRegistryKey(path, true);
                registryKey.DeleteValue(entryName);
            }
            catch (Exception e) when (e is SecurityException || e is UnauthorizedAccessException)
            {
                RunCommandLine(new[]
                {
                    "delete", path,
                    "/v", entryName,
                    "/f"
                }, true);
            }
        }
    }
}