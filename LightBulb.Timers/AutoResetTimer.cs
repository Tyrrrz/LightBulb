using System;
using System.Threading;

namespace LightBulb.Timers
{
    public class AutoResetTimer : IDisposable
    {
        private readonly Action _handler;
        private readonly Timer _internalTimer;

        private bool _isBusy;

        public AutoResetTimer(Action handler)
        {
            _handler = handler;
            _internalTimer = new Timer(_ => Tick(), null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        private void Tick()
        {
            if (_isBusy)
                return;

            _isBusy = true;
            _handler();
            _isBusy = false;
        }

        public AutoResetTimer Start(TimeSpan interval, TimeSpan initialTickDelay)
        {
            _internalTimer.Change(initialTickDelay, interval);
            return this;
        }

        public AutoResetTimer Start(TimeSpan interval) => Start(interval, interval);

        public AutoResetTimer Stop()
        {
            _internalTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            return this;
        }

        public void Dispose() => _internalTimer.Dispose();
    }
}