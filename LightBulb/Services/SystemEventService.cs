using System;
using System.Collections.Generic;
using System.Diagnostics;
using LightBulb.WindowsApi;

namespace LightBulb.Services
{
    public class SystemEventService : IDisposable
    {
        private readonly List<PowerSettingEvent> _powerSettingEvents = new List<PowerSettingEvent>();

        public event EventHandler? DisplayStateChanged;

        public SystemEventService()
        {
            RegisterAllEvents();
        }

        private void RegisterAllEvents()
        {
            // Display state changed
            var powerSettingEvent = PowerSettingEvent.Register(
                new Guid("6FE69556-704A-47A0-8F24-C28D936FDA47"),
                () => DisplayStateChanged?.Invoke(this, EventArgs.Empty)
            );

            if (powerSettingEvent != null)
                _powerSettingEvents.Add(powerSettingEvent);
            else
                Debug.WriteLine("Failed to register power setting event.");
        }

        private void UnregisterAllEvents()
        {
            // Dispose all registered events and remove them from the list
            foreach (var powerSettingEvent in _powerSettingEvents.ToArray())
            {
                powerSettingEvent.Dispose();
                _powerSettingEvents.Remove(powerSettingEvent);
            }
        }

        public void Dispose()
        {
            DisplayStateChanged = null;

            UnregisterAllEvents();
        }
    }
}
