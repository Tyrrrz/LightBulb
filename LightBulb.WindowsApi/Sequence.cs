using System;
using System.Threading;

namespace LightBulb.WindowsApi
{
    public class Sequence : IDisposable
    {
        private readonly System.Threading.Timer _internalTimer;
        private readonly TimeSpan _frameTime;
        private readonly int _frameCount;
        private readonly Action _action;

        private int _currentFrame;

        public Sequence(TimeSpan frameTime, int frameCount, Action action)
        {
            _frameTime = frameTime;
            _frameCount = frameCount;
            _action = action;
            _internalTimer =
                new System.Threading.Timer(_ => Tick(), null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        private void Tick()
        {
            if (_currentFrame++ < _frameCount)
                _action();
            else
                Stop();
        }

        public void Start()
        {
            _currentFrame = 0;
            _internalTimer.Change(TimeSpan.Zero, _frameTime);
        }

        public void Stop() => _internalTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

        public void Dispose() => _internalTimer.Dispose();
    }
}