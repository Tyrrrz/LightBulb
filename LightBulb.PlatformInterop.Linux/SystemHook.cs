using System;

namespace LightBulb.PlatformInterop;

public partial class SystemHook : IDisposable
{
    private SystemHook() { }

    public void Dispose() { }
}

public partial class SystemHook
{
    public static SystemHook? TryRegister(int hookId, Action callback) => null;
}

public partial class SystemHook
{
    public static class Ids
    {
        public static int ForegroundWindowChanged => 3;
    }
}
