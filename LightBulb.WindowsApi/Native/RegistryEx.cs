using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security;
using Microsoft.Win32;
using Tyrrrz.Extensions;

namespace LightBulb.WindowsApi.Native
{
    internal static class RegistryEx
    {
        public static RegistryKey GetRegistryKeyFromHiveName(string hiveName)
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
            throw new NotSupportedException($"Unsupported or invalid hive '{hiveName}'.");
        }

        public static string GetRegistryValueType(Type type)
        {
            if (type == typeof(int))
                return "REG_DWORD";

            if (type == typeof(string))
                return "REG_SZ";

            // Others are not supported because they're not used in this application
            throw new NotSupportedException($"Unsupported registry value type '{type}'.");
        }

        public static void RunRegistryCli(IReadOnlyList<string> arguments, bool asAdmin = false)
        {
            var processInfo = new ProcessStartInfo("reg");

            processInfo.ArgumentList.AddRange(arguments);

            if (asAdmin)
            {
                processInfo.UseShellExecute = true;
                processInfo.Verb = "runas";
            }

            using var process = Process.Start(processInfo);

            // This needs to ensure success but we don't really have a fallback plan anyway
            process?.WaitForExit(3000);
        }

        public static object? GetValueOrDefault(string path, string entryName)
        {
            var hiveName = path.SubstringUntil("\\");
            var relativePath = path.SubstringAfter("\\");

            using var registryKey = GetRegistryKeyFromHiveName(hiveName).OpenSubKey(relativePath, false);
            return registryKey?.GetValue(entryName);

            // No command line fallback because this one should never have security issues, hopefully
        }

        public static void SetValue(string path, string entryName, object entryValue)
        {
            try
            {
                var hiveName = path.SubstringUntil("\\");
                var relativePath = path.SubstringAfter("\\");

                using var registryKey = GetRegistryKeyFromHiveName(hiveName).CreateSubKey(relativePath, true);
                registryKey.SetValue(entryName, entryValue);
            }
            catch (Exception ex) when (ex is SecurityException || ex is UnauthorizedAccessException)
            {
                RunRegistryCli(new[]
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
                var hiveName = path.SubstringUntil("\\");
                var relativePath = path.SubstringAfter("\\");

                using var registryKey = GetRegistryKeyFromHiveName(hiveName).OpenSubKey(relativePath, true);
                registryKey?.DeleteValue(entryName, false);
            }
            catch (Exception ex) when (ex is SecurityException || ex is UnauthorizedAccessException)
            {
                RunRegistryCli(new[]
                {
                    "delete", path,
                    "/v", entryName,
                    "/f"
                }, true);
            }
        }
    }
}