using System;
using System.Collections.Generic;
using System.Diagnostics;
using LightBulb.WindowsApi;
using LightBulb.Models.PowerState;

namespace LightBulb.Services
{
    public class PowerBroadcastService
    {
        private List<PowerBroadcast> _broadcasts = new List<PowerBroadcast>();

        public void RegisterDisplayOn(Action handler)
        {
            RegisterBroadcast<Display>((Display) => 
            {
                if(Display.GetState() == Display.States.On)
                    handler.Invoke();
            });
        }

        public void RegisterBroadcast<T>(Action<T> handler) where T : PowerState, new()
        {
            Guid id = new T().Guid;

            var broadcast = PowerBroadcast.Register((data) => InvokeHandler(handler, data), id);

            if (broadcast != null)
                _broadcasts.Add(broadcast);
            else
                Debug.WriteLine("Failed to register for power broadcast.");

        }

        private void InvokeHandler<T>(Action<T> handler, byte data) where T : PowerState, new()
        {
            var arg = new T();
            arg.SetState(data);

            handler.Invoke(arg);
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
