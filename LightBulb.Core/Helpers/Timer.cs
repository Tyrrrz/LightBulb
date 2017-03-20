using System;

namespace LightBulb.Helpers
{
    /// <summary>
    /// Timer that runs on a separate thread and only triggers new events if the old ones have been handled
    /// </summary>
    public class Timer : IDisposable
    {
        private bool _isBusy;

        protected System.Timers.Timer InternalTimer { get; }

        /// <summary>
        /// Whether the timer should fire events
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// The amount of time between each timer tick
        /// </summary>
        public TimeSpan Interval
        {
            get { return TimeSpan.FromMilliseconds(InternalTimer.Interval); }
            set { InternalTimer.Interval = value.TotalMilliseconds; }
        }

        /// <summary>
        /// Triggered when the timer ticks
        /// </summary>
        public event EventHandler Tick;

        public Timer(TimeSpan interval)
        {
            InternalTimer = new System.Timers.Timer
            {
                AutoReset = true,
                Interval = interval.TotalMilliseconds
            };
            InternalTimer.Elapsed += (sender, args) => TimerTickInternal();
            InternalTimer.Start();
        }

        public Timer()
            : this(TimeSpan.FromMilliseconds(100))
        {            
        }

        ~Timer()
        {
            Dispose(false);
        }

        private void TimerTickInternal()
        {
            if (!IsEnabled || _isBusy) return;
            _isBusy = true;
            TimerTick();
            _isBusy = false;
        }

        protected virtual void TimerTick()
        {
            Tick?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                InternalTimer.Dispose();
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