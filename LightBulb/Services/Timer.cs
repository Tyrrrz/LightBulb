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
        public bool IsEnabled
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
        public TimeSpan Interval { get; set; }

        /// <summary>
        /// Timer tick event
        /// </summary>
        public event EventHandler Tick;

        public Timer(TimeSpan interval)
        {
            Interval = interval;

            InternalTimer = new System.Timers.Timer { AutoReset = false };
            InternalTimer.Elapsed += (sender, args) => TimerTickInternal();
        }

        public Timer() : this(TimeSpan.Zero)
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
            InternalTimer.Interval = Interval.TotalMilliseconds;
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