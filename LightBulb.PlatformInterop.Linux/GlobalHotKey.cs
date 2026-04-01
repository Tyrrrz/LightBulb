using System;

namespace LightBulb.PlatformInterop;

public partial class GlobalHotKey : IDisposable
{
    private GlobalHotKey() { }

    public void Dispose() { }
}

public partial class GlobalHotKey
{
    public static GlobalHotKey? TryRegister(int virtualKey, int modifiers, Action callback) =>
        null;
}
