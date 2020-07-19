using System;
using System.Threading;

namespace LightBulb.WindowsApi.Timers
{
    internal class AutoResetTimer : ITimer
    {
        private readonly object _lock = new object();

        private readonly Action _handler;
        private readonly TimeSpan _firstTickInterval;
        private readonly TimeSpan _interval;

        private readonly System.Threading.Timer _internalTimer;

        private bool _isActive;
        private bool _isBusy;
        private bool _isDisposed;

        public AutoResetTimer(TimeSpan firstTickInterval, TimeSpan interval, Action handler)
        {
            _firstTickInterval = firstTickInterval;
            _interval = interval;
            _handler = handler;

            _internalTimer = new System.Threading.Timer(
                _ => Tick(),
                null,
                Timeout.InfiniteTimeSpan,
                Timeout.InfiniteTimeSpan
            );
        }

        private void Tick()
        {
            // Prevent multiple reentry
            if (_isBusy)
                return;

            lock (_lock)
            {
                // Prevent executing when already disposed (race condition)
                if (_isDisposed)
                    return;

                try
                {
                    _isBusy = true;
                    _handler();
                }
                finally
                {
                    _isBusy = false;
                }
            }
        }

        public void Start()
        {
            if (_isActive) return;

            _internalTimer.Change(_firstTickInterval, _interval);
            _isActive = true;
        }

        public void Stop()
        {
            if (!_isActive) return;

            _internalTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            _isActive = false;
        }

        public void Dispose()
        {
            // Lock so that tick doesn't trigger after dispose (bad idea?)
            lock (_lock)
            {
                _isActive = false;
                _isDisposed = true;
                _internalTimer.Dispose();
            }
        }
    }
}