using System;
using System.Collections.Generic;
using System.Security;
using System.Threading;
using LightBulb.PlatformInterop.Utils;
using Microsoft.Win32;
using PowerKit.Extensions;

namespace LightBulb.PlatformInterop;

public class RegistrySwitch<T>(RegistryHive hive, string keyName, string entryName, T enabledValue)
    where T : notnull
{
    private readonly Lock _lock = new();

    public bool IsSet
    {
        get
        {
            using (_lock.EnterScope())
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
            using (_lock.EnterScope())
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
                    {
                        Reg.SetValue(hive.Moniker + '\\' + keyName, entryName, enabledValue);
                    }
                    else
                    {
                        Reg.DeleteValue(hive.Moniker + '\\' + keyName, entryName);
                    }
                }
            }
        }
    }
}
