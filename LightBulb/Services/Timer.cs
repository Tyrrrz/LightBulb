using System;

namespace LightBulb.Services
{
    /// <summary>
    /// Timer that runs on a separate thread and only triggers new events if the old ones have been handled
    /// </summary>
    public class Timer : IDisposable
    {
        private bool _isBusy;
        private bool _isEnabled;

        protected System.Timers.Timer InternalTimer { get; }

        /// <summary>
        /// Gets or sets whether the timer should fire events
        /// </summary>
        public virtual bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (IsEnabled == value) return;
                _isEnabled = value;

                // If timer is busy - don't touch it
                if (_isBusy) return;

                // Otherwise start it or stop it
                if (value)
                    Start();
                else
                    Stop();
            }
        }

        /// <summary>
        /// The amount of time between each timer tick
        /// </summary>
        public virtual TimeSpan Interval
        {
            get { return TimeSpan.FromMilliseconds(InternalTimer.Interval); }
            set { InternalTimer.Interval = value.TotalMilliseconds; }
        }

        /// <summary>
        /// Timer tick event
        /// </summary>
        public event EventHandler Tick;

        public Timer(TimeSpan interval)
        {
            InternalTimer = new System.Timers.Timer
            {
                AutoReset = false,
                Interval = interval.TotalMilliseconds
            };
            InternalTimer.Elapsed += (sender, args) => TimerTickInternal();
        }

        public Timer() : this(TimeSpan.FromMilliseconds(100))
        {
            
        }

        private void TimerTickInternal()
        {
            _isBusy = true;
            InternalTimer.Enabled = false;
            TimerTick();
            InternalTimer.Enabled = IsEnabled;
            _isBusy = false;
        }

        protected virtual void Start()
        {
            InternalTimer.Start();
        }

        protected virtual void Stop()
        {
            InternalTimer.Stop();
        }

        protected virtual void TimerTick()
        {
            Tick?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            InternalTimer.Dispose();
        }
    }
}