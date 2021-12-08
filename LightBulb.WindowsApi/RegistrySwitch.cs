using System.Collections.Generic;
using LightBulb.WindowsApi.Native;

namespace LightBulb.WindowsApi;

public class RegistrySwitch
{
    private readonly string _path;
    private readonly string _entryName;
    private readonly object _enabledValue;

    public bool IsSet
    {
        get
        {
            var actualValue = RegistryEx.GetValueOrDefault(_path, _entryName);
            return EqualityComparer<object>.Default.Equals(_enabledValue, actualValue);
        }
        set
        {
            if (value && !IsSet)
            {
                RegistryEx.SetValue(_path, _entryName, _enabledValue);
            }
            else if (!value && IsSet)
            {
                RegistryEx.DeleteValue(_path, _entryName);
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