using System;
using System.Collections.Generic;
using System.Security;
using LightBulb.WindowsApi.Utils;
using LightBulb.WindowsApi.Utils.Extensions;
using Microsoft.Win32;

namespace LightBulb.WindowsApi;

public class RegistrySwitch<T>
    where T : notnull
{
    private readonly RegistryHive _hive;
    private readonly string _keyName;
    private readonly string _entryName;
    private readonly T _enabledValue;

    private readonly object _lock = new();

    public bool IsSet
    {
        get
        {
            lock (_lock)
            {
                // This should always be accessible without elevation
                var value = _hive.OpenKey().OpenSubKey(_keyName, false)?.GetValue(_entryName);
                if (value is null)
                    return false;

                return EqualityComparer<T>.Default.Equals(_enabledValue, (T)value);
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
                    var key = _hive.OpenKey().CreateSubKey(_keyName, true);

                    if (value)
                        key.SetValue(_entryName, _enabledValue);
                    else
                        key.DeleteValue(_entryName);
                }
                catch (Exception ex) when (ex is SecurityException or UnauthorizedAccessException)
                {
                    // Run reg.exe with elevation
                    if (value)
                        Reg.SetValue(
                            _hive.GetShortMoniker() + '\\' + _keyName,
                            _entryName,
                            _enabledValue
                        );
                    else
                        Reg.DeleteValue(_hive.GetShortMoniker() + '\\' + _keyName, _entryName);
                }
            }
        }
    }

    public RegistrySwitch(RegistryHive hive, string keyName, string entryName, T enabledValue)
    {
        _hive = hive;
        _keyName = keyName;
        _entryName = entryName;
        _enabledValue = enabledValue;
    }
}
