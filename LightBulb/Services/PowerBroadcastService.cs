using System;
using System.Collections.Generic;
using System.Diagnostics;
using LightBulb.WindowsApi;

namespace LightBulb.Services
{
    public class PowerBroadcastService
    {
        public event EventHandler? DisplayStateChanged;

        private readonly List<PowerSettingEvent> powerSettingEvents = new List<PowerSettingEvent>();

        public PowerBroadcastService()
        {
            var _displayStateBroadcast = PowerSettingEvent.Register(new Guid("6FE69556-704A-47A0-8F24-C28D936FDA47"), () => DisplayStateChanged?.Invoke(this, new EventArgs()));
            if(_displayStateBroadcast != null) powerSettingEvents.Add(_displayStateBroadcast);
        }
    }
}
