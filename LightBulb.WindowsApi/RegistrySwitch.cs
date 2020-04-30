using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security;
using Microsoft.Win32;
using Tyrrrz.Extensions;

namespace LightBulb.WindowsApi
{
    public partial class RegistrySwitch
    {
        private readonly string _path;
        private readonly string _entryName;
        private readonly object _expectedValue;

        public RegistrySwitch(string path, string entryName, object expectedValue)
        {
            _path = path;
            _entryName = entryName;
            _expectedValue = expectedValue;
        }

        public bool IsEnabled()
        {
            var actualValue = GetValueOrDefault(_path, _entryName);
            return Equals(_expectedValue, actualValue);
        }

        public void Enable()
        {
            if (!IsEnabled())
                SetValue(_path, _entryName, _expectedValue);
        }

        public void Disable()
        {
            if (IsEnabled())
                DeleteValue(_path, _entryName);
        }

        public void SetEnabled(bool isEnabled)
        {
            if (isEnabled)
                Enable();
            else
                Disable();
        }
    }

    public partial class RegistrySwitch
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
            throw new NotSupportedException($"Unsupported or invalid hive '{hiveName}'.");
        }

        private static string GetRegistryValueType(Type type)
        {
            if (type == typeof(int))
                return "REG_DWORD";

            if (type == typeof(string))
                return "REG_SZ";

            // Others are not supported because they're not used in this application
            throw new NotSupportedException($"Unsupported registry value type '{type}'.");
        }

        private static void RunRegistryCli(IReadOnlyList<string> arguments, bool asAdmin = false)
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

        private static object? GetValueOrDefault(string path, string entryName)
        {
            // No command line fallback because this one should never have security issues, hopefully
            using var registryKey = GetRegistryKey(path, false);
            return registryKey?.GetValue(entryName);
        }

        private static void SetValue(string path, string entryName, object entryValue)
        {
            try
            {
                using var registryKey = GetRegistryKey(path, true);
                registryKey.SetValue(entryName, entryValue);
            }
            catch (Exception e) when (e is SecurityException || e is UnauthorizedAccessException)
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

        private static void DeleteValue(string path, string entryName)
        {
            try
            {
                using var registryKey = GetRegistryKey(path, true);
                registryKey.DeleteValue(entryName, false);
            }
            catch (Exception e) when (e is SecurityException || e is UnauthorizedAccessException)
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