using System;
using System.Threading;

namespace LightBulb.Timers
{
    public class AutoResetTimer : IDisposable
    {
        private readonly object _lock = new object();

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
            // Prevent multiple reentry
            lock (_lock)
            {
                if (_isBusy) return;
                _isBusy = true;
            }

            // Execute handler and reset busy state
            try
            {
                _handler();
            }
            finally
            {
                _isBusy = false;
            }
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