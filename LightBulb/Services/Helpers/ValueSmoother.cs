using System;
using Tyrrrz.Extensions;

namespace LightBulb.Services.Helpers
{
    public class ValueSmoother : IDisposable
    {
        private readonly Timer _timer;
        private Action<double> _setter;
        private double _current;
        private double _increment;
        private double _final;

        public event EventHandler Finished;

        public ValueSmoother()
        {
            _timer = new Timer(TimeSpan.FromMilliseconds(50));
            _timer.Tick += (sender, args) => Tick();
        }

        private void Tick()
        {
            bool isIncreasing = _increment > 0;

            _current += _increment;
            _current = isIncreasing ? _current.ClampMax(_final) : _current.ClampMin(_final);
            _setter(_current);

            // Ending condition
            if ((isIncreasing && _current >= _final) || (!isIncreasing && _current <= _final))
            {
                _timer.IsEnabled = false;
                Finished?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Smoothly changes the value of a property over time
        /// </summary>
        public void Set(double from, double to, Action<double> setter, TimeSpan duration)
        {
            if (setter == null)
                throw new ArgumentNullException(nameof(setter));
            if (duration == TimeSpan.Zero)
            {
                setter(to);
                return;
            }

            _timer.IsEnabled = false;

            _current = from;
            _final = to;
            _setter = setter;

            _increment = (to - from)*_timer.Interval.TotalMilliseconds/duration.TotalMilliseconds;
            _timer.IsEnabled = true;
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