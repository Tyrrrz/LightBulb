using System;
using System.Collections.Generic;

namespace LightBulb.PlatformInterop;

public partial class DisplayStateWatcher : IDisposable
{
    private readonly List<IDisposable> _registrations;

    public DisplayStateWatcher(Action onGammaInvalidated, Action onDeviceContextInvalidated)
    {
        var registrations = new List<IDisposable>();

        void AddIfNotNull(IDisposable? d)
        {
            if (d is not null)
                registrations.Add(d);
        }

        // Invalidate gamma when the foreground window changes (another app may have reset it)
        // https://github.com/Tyrrrz/LightBulb/issues/223
        AddIfNotNull(
            SystemHook.TryRegister(SystemHook.Ids.ForegroundWindowChanged, onGammaInvalidated)
        );

        // Invalidate gamma when any power-related setting changes
        AddIfNotNull(
            PowerSettingNotification.TryRegister(
                PowerSettingNotification.Ids.ConsoleDisplayStateChanged,
                onGammaInvalidated
            )
        );
        AddIfNotNull(
            PowerSettingNotification.TryRegister(
                PowerSettingNotification.Ids.PowerSavingStatusChanged,
                onGammaInvalidated
            )
        );
        AddIfNotNull(
            PowerSettingNotification.TryRegister(
                PowerSettingNotification.Ids.SessionDisplayStatusChanged,
                onGammaInvalidated
            )
        );
        AddIfNotNull(
            PowerSettingNotification.TryRegister(
                PowerSettingNotification.Ids.MonitorPowerStateChanged,
                onGammaInvalidated
            )
        );
        AddIfNotNull(
            PowerSettingNotification.TryRegister(
                PowerSettingNotification.Ids.AwayModeChanged,
                onGammaInvalidated
            )
        );

        // Invalidate device contexts when the display configuration changes
        registrations.Add(
            SystemEvent.Register(SystemEvent.Ids.DisplayChanged, onDeviceContextInvalidated)
        );
        registrations.Add(
            SystemEvent.Register(SystemEvent.Ids.PaletteChanged, onDeviceContextInvalidated)
        );
        registrations.Add(
            SystemEvent.Register(SystemEvent.Ids.SettingsChanged, onDeviceContextInvalidated)
        );
        registrations.Add(
            SystemEvent.Register(SystemEvent.Ids.SystemColorsChanged, onDeviceContextInvalidated)
        );

        _registrations = registrations;
    }

    public void Dispose()
    {
        foreach (var registration in _registrations)
            registration.Dispose();
        _registrations.Clear();
    }
}

public partial class DisplayStateWatcher
{
    public static DisplayStateWatcher Create(
        Action onGammaInvalidated,
        Action onDeviceContextInvalidated
    ) => new(onGammaInvalidated, onDeviceContextInvalidated);
}
