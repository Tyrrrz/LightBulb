using System;

namespace LightBulb.PlatformInterop;

public partial class SystemEvent : IDisposable
{
    private SystemEvent() { }

    public void Dispose() { }
}

public partial class SystemEvent
{
    public static SystemEvent Register(int eventId, Action callback) => new();
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
