using Microsoft.Win32;

namespace LightBulb.PlatformInterop;

public class RegistrySwitch<T>
    where T : notnull
{
    public RegistrySwitch(RegistryHive hive, string keyName, string entryName, T enabledValue) { }

    public bool IsSet
    {
        get => false;
        set { }
    }
}
