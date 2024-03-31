using System;

namespace LightBulb.PlatformInterop;

public partial class SystemEvent(int eventId, Action callback) : IDisposable
{
    private readonly IDisposable _wndProcRegistration = WndProc2.Listen(eventId, _ => callback());

    public void Dispose() => _wndProcRegistration.Dispose();
}

public partial class SystemEvent
{
    public static SystemEvent Register(int eventId, Action callback) => new(eventId, callback);
}

public partial class SystemEvent
{
    public static class Ids
    {
        public static int DisplayChanged => 126;
        public static int PaletteChanged => 785;
        public static int SystemColorsChanged => 21;
        public static int SettingsChanged => 26;
    }
}
