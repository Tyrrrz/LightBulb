using System;
using LightBulb.WindowsApi.Internal;
using Microsoft.Win32;

namespace LightBulb.WindowsApi.Events
{
    public enum SystemEventType
    {
        DisplayStateChanged,
        DisplaySettingsChanged
    }

    public static class SystemEvent
    {
        public static IDisposable? TryRegisterHotKey(int virtualKey, int modifiers, Action handler) =>
            GlobalHotKey.TryRegister(virtualKey, modifiers, handler);

        public static IDisposable? TryRegister(SystemEventType type, Action handler)
        {
            if (type == SystemEventType.DisplayStateChanged)
            {
                return Disposable.Aggregate(
                    PowerEvent.TryRegister(PowerEvent.ConsoleDisplayStateId, handler),
                    PowerEvent.TryRegister(PowerEvent.MonitorPowerOnId, handler),
                    PowerEvent.TryRegister(PowerEvent.PowerSavingStatusId, handler),
                    PowerEvent.TryRegister(PowerEvent.SessionDisplayStatusId, handler),
                    PowerEvent.TryRegister(PowerEvent.AwayModeId, handler)
                );
            }

            if (type == SystemEventType.DisplaySettingsChanged)
            {
                var adapter = new EventDelegateAdapter(handler);

                SystemEvents.DisplaySettingsChanging += adapter.HandleEvent;
                SystemEvents.DisplaySettingsChanged += adapter.HandleEvent;

                return Disposable.Create(() =>
                {
                    SystemEvents.DisplaySettingsChanging -= adapter.HandleEvent;
                    SystemEvents.DisplaySettingsChanged -= adapter.HandleEvent;
                });
            }

            throw new ArgumentOutOfRangeException(nameof(type));
        }
    }
}