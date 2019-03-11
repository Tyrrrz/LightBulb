using System;

namespace LightBulb.WindowsApi
{
    public class Timer : IDisposable
    {
        private readonly System.Threading.Timer _internalTimer;

        public Timer(TimeSpan initialDelay, TimeSpan interval, Action handler)
        {
            _internalTimer = new System.Threading.Timer(_ => handler(), null, initialDelay, interval);
        }

        public Timer(TimeSpan interval, Action handler)
            : this(TimeSpan.Zero, interval, handler)
        {
        }

        public void Change(TimeSpan initialDelay, TimeSpan interval) => _internalTimer.Change(initialDelay, interval);

        public void Change(TimeSpan interval) => Change(TimeSpan.Zero, interval);

        public void Dispose() => _internalTimer.Dispose();
    }
}