using System;
using LightBulb.WindowsApi.Internal;

namespace LightBulb.WindowsApi
{
    public partial class SystemEvent : IDisposable
    {
        private readonly IDisposable _wndProcRegistration;

        public int EventId { get; }

        public Action Callback { get; }

        public SystemEvent(int eventId, Action callback)
        {
            EventId = eventId;
            Callback = callback;

            _wndProcRegistration = WndProc.Listen(eventId, m => callback());
        }

        // There are no native resources, but it's pretty important to unregister from wndproc
        ~SystemEvent() => Dispose();

        public void Dispose()
        {
            _wndProcRegistration.Dispose();
            GC.SuppressFinalize(this);
        }
    }

    public partial class SystemEvent
    {
        public static int DisplayChangedId { get; } = 126;

        public static int PaletteChangedId { get; } = 785;

        public static int SystemColorsChangedId { get; } = 12;

        public static int SettingsChangedId { get; } = 26;

        public static SystemEvent Register(int eventId, Action callback) =>
            new SystemEvent(eventId, callback);
    }
}