using System;

namespace LightBulb.WindowsApi
{
    public class FixedDurationAutoResetTimer : IDisposable
    {
        private readonly TimeSpan _frameTime;
        private readonly TimeSpan _duration;
        private readonly TimeSpan _delta;
        private readonly Action<TimeSpan> _action;

        private readonly AutoResetTimer _internalTimer;

        private TimeSpan _elapsed;

        public FixedDurationAutoResetTimer(TimeSpan frameTime, TimeSpan duration, Action<TimeSpan> action)
        {
            _frameTime = frameTime;
            _duration = duration;
            _delta = TimeSpan.FromMilliseconds(_duration.TotalMilliseconds / _frameTime.TotalMilliseconds);
            _action = action;

            _internalTimer = new AutoResetTimer(Tick);
        }

        public FixedDurationAutoResetTimer(TimeSpan duration, Action<TimeSpan> action)
            : this(TimeSpan.FromMilliseconds(50), duration, action)
        {
        }

        public FixedDurationAutoResetTimer(TimeSpan duration, Action action)
            : this(duration, _ => action())
        {
        }

        private void Tick()
        {
            if (_elapsed < _duration)
            {
                _action(_elapsed);
                _elapsed += _delta;
            }
            else
            {
                Stop();
            }
        }

        public FixedDurationAutoResetTimer Start()
        {
            _elapsed = TimeSpan.Zero;
            _internalTimer.Start(_frameTime);

            return this;
        }

        public FixedDurationAutoResetTimer Stop()
        {
            _internalTimer.Stop();
            return this;
        }

        public void Dispose() => _internalTimer.Dispose();
    }
}