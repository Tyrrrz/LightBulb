using System;
using System.Collections.Generic;
using LightBulb.WindowsApi;

namespace LightBulb.Services
{
    class PowerBroadcastService
    {
        private List<PowerBroadcast> _broadcasts = new List<PowerBroadcast>();

        public void RegisterBroadcast<T>(Action handler) where T : PowerState, new()
        {
            Guid id = new T().Guid;
            var broadcast = new PowerBroadcast(handler, id);
            _broadcasts.Add(broadcast);
        }

        public void UnregisterAllBroadcasts()
        {
            foreach (PowerBroadcast broadcast in _broadcasts)
                broadcast.Dispose();

            _broadcasts.Clear();
        }

        public void Dispose() => UnregisterAllBroadcasts();
    }
}
