using System;
using System.Threading;

namespace LightBulb.Timers
{
    public class ManualResetTimer : IDisposable
    {
        private readonly AutoResetTimer _internalTimer;

        public ManualResetTimer(Action handler)
        {
            _internalTimer = new AutoResetTimer(handler);
        }

        public ManualResetTimer Start(TimeSpan delay)
        {
            _internalTimer.Start(Timeout.InfiniteTimeSpan, delay);
            return this;
        }

        public ManualResetTimer Stop()
        {
            _internalTimer.Stop();
            return this;
        }

        public void Dispose() => _internalTimer.Dispose();
    }
}