using System;
using System.Threading;

namespace LightBulb.WindowsApi
{
    public class DelayedActionController : IDisposable
    {
        private readonly System.Threading.Timer _internalTimer;
        private Action _action;

        public DelayedActionController()
        {
            _internalTimer = new System.Threading.Timer(_ => Tick(), null, 
                Timeout.InfiniteTimeSpan,
                Timeout.InfiniteTimeSpan);
        }

        private void Tick() => _action?.Invoke();

        public void Unschedule()
        {
            // Disable timer first then reset action
            _internalTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            _action = null;
        }

        public void Schedule(TimeSpan delay, Action action)
        {
            // Unschedule first to prevent race conditions
            Unschedule();

            // Assign new action and change timer to new delay
            _action = action;
            _internalTimer.Change(delay, Timeout.InfiniteTimeSpan);
        }

        public void Dispose() => _internalTimer.Dispose();
    }
}