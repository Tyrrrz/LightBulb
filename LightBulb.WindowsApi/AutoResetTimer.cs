using System;
using System.Threading;

namespace LightBulb.WindowsApi
{
    public class AutoResetTimer : IDisposable
    {
        private readonly Timer _internalTimer;

        public AutoResetTimer(Action action)
        {
            _internalTimer = new Timer(_ => action(), null,
                Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
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

        public void Dispose() => _internalTimer.Dispose();
    }
}