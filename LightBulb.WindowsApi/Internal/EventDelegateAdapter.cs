using System;

namespace LightBulb.WindowsApi.Internal
{
    internal class EventDelegateAdapter
    {
        private readonly Action _handler;

        public EventDelegateAdapter(Action handler) => _handler = handler;

        public void HandleEvent(object? sender, EventArgs? args) => _handler();
    }
}