using System;
using System.Threading;

namespace LightBulb.WindowsApi.Timers
{
    public static class Timer
    {
        public static ITimer Create(TimeSpan interval, Action handler) =>
            new AutoResetTimer(TimeSpan.Zero, interval, handler);

        public static IDisposable QueueDelayedAction(TimeSpan delay, Action handler)
        {
            var timer = new AutoResetTimer(delay, Timeout.InfiniteTimeSpan, handler);
            timer.Start();

            return timer;
        }
    }
}