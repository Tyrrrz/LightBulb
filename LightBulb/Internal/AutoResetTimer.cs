using System;
using System.Threading;

namespace LightBulb.Internal
{
    internal class AutoResetTimer : IDisposable
    {
        private readonly object _lock = new object();

        private readonly Action _tick;
        private readonly Timer _internalTimer;

        private bool _isBusy;
        private bool _isDisposed;

        public AutoResetTimer(Action tick)
        {
            _tick = tick;
            _internalTimer = new Timer(_ => HandleTick(), null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        private void HandleTick()
        {
            // Prevent multiple reentry
            if (_isBusy)
                return;

            lock (_lock)
            {
                // Prevent executing when already disposed (race condition)
                if (_isDisposed)
                    return;

                _isBusy = true;

                try
                {
                    _tick();
                }
                finally
                {
                    _isBusy = false;
                }
            }
        }

        public AutoResetTimer Start(TimeSpan initialTickDelay, TimeSpan interval)
        {
            _internalTimer.Change(initialTickDelay, interval);
            return this;
        }

        public AutoResetTimer Start(TimeSpan interval) => Start(TimeSpan.Zero, interval);

        public AutoResetTimer Stop()
        {
            _internalTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            return this;
        }

        public void Dispose()
        {
            // Lock so that tick doesn't trigger after dispose (bad idea?)
            lock (_lock)
            {
                _isDisposed = true;
                _internalTimer.Dispose();
            }
        }
    }
}