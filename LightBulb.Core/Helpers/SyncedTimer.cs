using System;
using Tyrrrz.Extensions;

namespace LightBulb.Helpers
{
    /// <summary>
    /// Timer that triggers recurring events at specified time and interval, regardless of when it was started
    /// </summary>
    public class SyncedTimer : IDisposable
    {
        private readonly Timer _timer;
        private DateTime _firstTickDateTime;
        private TimeSpan _interval;

        /// <summary>
        /// Whether the timer should fire events
        /// </summary>
        public bool IsEnabled
        {
            get => _timer.IsEnabled;
            set
            {
                if (value)
                    SyncInterval();
                _timer.IsEnabled = value;
            }
        }

        /// <summary>
        /// The amount of time between each timer tick
        /// </summary>
        public TimeSpan Interval
        {
            get => _interval;
            set
            {
                if (value <= TimeSpan.Zero)
                    throw new ArgumentOutOfRangeException(nameof(Interval));

                _interval = value;
                SyncInterval();
            }
        }

        /// <summary>
        /// The first time the timer should fire an event
        /// </summary>
        public DateTime FirstTickDateTime
        {
            get => _firstTickDateTime;
            set
            {
                _firstTickDateTime = value;
                SyncInterval();
            }
        }

        /// <summary>
        /// Triggered when the timer ticks
        /// </summary>
        public event EventHandler Tick;

        public SyncedTimer(TimeSpan interval, DateTime firstTickDateTime)
        {
            _timer = new Timer();
            _timer.Tick += (sender, args) =>
            {
                Tick?.Invoke(this, EventArgs.Empty);
                SyncInterval();
            };

            _interval = interval;
            _firstTickDateTime = firstTickDateTime;

            SyncInterval();
        }

        public SyncedTimer(TimeSpan interval)
            : this(interval, DateTime.Today)
        {
        }

        public SyncedTimer()
            : this(TimeSpan.FromMilliseconds(100), DateTime.Today)
        {
        }

        ~SyncedTimer()
        {
            Dispose(false);
        }

        private void SyncInterval()
        {
            var now = DateTime.Now;

            if (now < FirstTickDateTime)
            {
                _timer.Interval = FirstTickDateTime - now;
            }
            else
            {
                var timePassed = now - FirstTickDateTime;
                double totalTicks = timePassed.TotalMilliseconds / Interval.TotalMilliseconds;
                double msUntilNextTick = (Math.Ceiling(totalTicks) - totalTicks) * Interval.TotalMilliseconds;
                msUntilNextTick = msUntilNextTick.ClampMin(1);
                _timer.Interval = TimeSpan.FromMilliseconds(msUntilNextTick);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timer.Dispose();
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}