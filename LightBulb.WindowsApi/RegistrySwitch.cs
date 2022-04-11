using System.Collections.Generic;
using LightBulb.WindowsApi.Native;

namespace LightBulb.WindowsApi;

public class RegistrySwitch
{
    private readonly string _path;
    private readonly string _entryName;
    private readonly object _enabledValue;

    private readonly object _lock = new();

    public bool IsSet
    {
        get
        {
            lock (_lock)
            {
                var actualValue = RegistryEx.GetValueOrDefault(_path, _entryName);
                return EqualityComparer<object>.Default.Equals(_enabledValue, actualValue);
            }
        }
        set
        {
            lock (_lock)
            {
                // Avoid unnecessary changes
                if (IsSet == value)
                    return;

                if (value)
                {
                    RegistryEx.SetValue(_path, _entryName, _enabledValue);
                }
                else
                {
                    RegistryEx.DeleteValue(_path, _entryName);
                }
            }
        }
    }

    public RegistrySwitch(string path, string entryName, object enabledValue)
    {
        _path = path;
        _entryName = entryName;
        _enabledValue = enabledValue;
    }
}