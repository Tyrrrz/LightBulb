using System;
using Tyrrrz.Extensions;

namespace LightBulb.Services.Helpers
{
    public class ValueSmoother : IDisposable
    {
        private readonly Timer _timer;
        private Action<double> _setter;
        private double _increment;
        private double _final;

        /// <summary>
        /// Whether a valus is being smoothed right now
        /// </summary>
        public bool IsActive => _timer.IsEnabled;

        /// <summary>
        /// Current value
        /// </summary>
        public double Current { get; private set; }

        public event EventHandler Finished;

        public ValueSmoother()
        {
            _timer = new Timer(TimeSpan.FromMilliseconds(50));
            _timer.Tick += (sender, args) => Tick();
        }

        private void Tick()
        {
            lock (_timer)
            {
                bool isIncreasing = _increment > 0;

                Current += _increment;
                Current = isIncreasing ? Current.ClampMax(_final) : Current.ClampMin(_final);
                _setter(Current);

                // Ending condition
                if ((isIncreasing && Current >= _final) || (!isIncreasing && Current <= _final))
                {
                    _timer.IsEnabled = false;
                    Finished?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Smoothly changes the value of a property over time
        /// </summary>
        public void Set(double from, double to, Action<double> setter, TimeSpan duration)
        {
            _timer.IsEnabled = false;

            if (setter == null)
                throw new ArgumentNullException(nameof(setter));
            if (duration < _timer.Interval)
                duration = _timer.Interval;

            lock (_timer)
            {
                Current = from;
                _final = to;
                _setter = setter;

                _increment = (to - from)*_timer.Interval.TotalMilliseconds/duration.TotalMilliseconds;
                _timer.IsEnabled = true;
            }
        }

        /// <summary>
        /// Stops the current transition, if active
        /// </summary>
        public void Stop()
        {
            _timer.IsEnabled = false;
        }

        public virtual void Dispose()
        {
            _timer.Dispose();
        }
    }
}