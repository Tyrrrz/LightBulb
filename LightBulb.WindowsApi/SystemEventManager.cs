using System;
using System.Collections.Generic;
using LightBulb.WindowsApi.Internal.Extensions;
using Microsoft.Win32;

namespace LightBulb.WindowsApi
{
    public class SystemEventManager : IDisposable
    {
        public event EventHandler? DisplaySettingsChanged;
        public event EventHandler? DisplayStateChanged;

        private readonly List<PowerSettingEvent> _powerSettingEvents = new List<PowerSettingEvent>();

        public SystemEventManager()
        {
            SystemEvents.DisplaySettingsChanging += SystemEventsOnDisplaySettingsChanging;
            SystemEvents.DisplaySettingsChanged += SystemEventsOnDisplaySettingsChanged;

            _powerSettingEvents.AddIfNotNull(PowerSettingEvent.TryRegister(
                Guid.Parse("6FE69556-704A-47A0-8F24-C28D936FDA47"),
                () => DisplayStateChanged?.Invoke(this, EventArgs.Empty))
            );
        }

        private void SystemEventsOnDisplaySettingsChanging(object? sender, EventArgs e) =>
            DisplaySettingsChanged?.Invoke(this, EventArgs.Empty);

        private void SystemEventsOnDisplaySettingsChanged(object? sender, EventArgs e) =>
            DisplaySettingsChanged?.Invoke(this, EventArgs.Empty);

        public void Dispose()
        {
            SystemEvents.DisplaySettingsChanging -= SystemEventsOnDisplaySettingsChanging;
            SystemEvents.DisplaySettingsChanged -= SystemEventsOnDisplaySettingsChanged;

            foreach (var powerEvent in _powerSettingEvents.ToArray())
            {
                powerEvent.Dispose();
                _powerSettingEvents.Remove(powerEvent);
            }
        }
    }
}