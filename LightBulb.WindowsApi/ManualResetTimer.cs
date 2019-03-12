using System;
using System.Threading;

namespace LightBulb.WindowsApi
{
    public class ManualResetTimer : IDisposable
    {
        private readonly Timer _internalTimer;

        public ManualResetTimer(Action action)
        {
            _internalTimer = new Timer(_ => action(), null,
                Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        public ManualResetTimer Start(TimeSpan delay)
        {
            _internalTimer.Change(delay, Timeout.InfiniteTimeSpan);
            return this;
        }

        public ManualResetTimer Stop()
        {
            _internalTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            return this;
        }

        public void Dispose() => _internalTimer.Dispose();
    }
}