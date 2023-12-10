using System;
using System.Collections.Generic;
using System.Security;
using LightBulb.WindowsApi.Utils;
using LightBulb.WindowsApi.Utils.Extensions;
using Microsoft.Win32;

namespace LightBulb.WindowsApi;

public class RegistrySwitch<T>(RegistryHive hive, string keyName, string entryName, T enabledValue)
    where T : notnull
{
    private readonly object _lock = new();

    public bool IsSet
    {
        get
        {
            lock (_lock)
            {
                // This should always be accessible without elevation
                var value = hive.OpenKey().OpenSubKey(keyName, false)?.GetValue(entryName);
                if (value is null)
                    return false;

                return EqualityComparer<T>.Default.Equals(enabledValue, (T)value);
            }
        }
        set
        {
            lock (_lock)
            {
                // Avoid unnecessary changes
                if (IsSet == value)
                    return;

                try
                {
                    var key = hive.OpenKey().CreateSubKey(keyName, true);

                    if (value)
                        key.SetValue(entryName, enabledValue);
                    else
                        key.DeleteValue(entryName);
                }
                catch (Exception ex) when (ex is SecurityException or UnauthorizedAccessException)
                {
                    // Run reg.exe with elevation
                    if (value)
                        Reg.SetValue(
                            hive.GetShortMoniker() + '\\' + keyName,
                            entryName,
                            enabledValue
                        );
                    else
                        Reg.DeleteValue(hive.GetShortMoniker() + '\\' + keyName, entryName);
                }
            }
        }
    }
}
