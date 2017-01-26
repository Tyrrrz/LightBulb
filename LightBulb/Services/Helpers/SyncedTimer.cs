using System;
using Tyrrrz.Extensions;

namespace LightBulb.Services.Helpers
{
    /// <summary>
    /// Timer that synchronizes its interval with the day time
    /// </summary>
    public class SyncedTimer : Timer
    {
        private DateTime _firstTickDateTime;
        private TimeSpan _interval;

        public override bool IsEnabled
        {
            get { return base.IsEnabled; }
            set
            {
                SyncInterval();
                base.IsEnabled = value;
            }
        }

        /// <summary>
        /// The first time the timer should fire an event
        /// </summary>
        public DateTime FirstTickDateTime
        {
            get { return _firstTickDateTime; }
            set
            {
                _firstTickDateTime = value;
                SyncInterval();
            }
        }

        /// <inheritdoc />
        public sealed override TimeSpan Interval
        {
            get { return _interval; }
            set
            {
                _interval = value;
                SyncInterval();
            }
        }

        public SyncedTimer(DateTime firstTickDateTime, TimeSpan interval)
        {
            FirstTickDateTime = firstTickDateTime;
            Interval = interval;
        }

        public SyncedTimer(TimeSpan interval)
            : this(DateTime.Today, interval)
        {
        }

        public SyncedTimer()
            : this(DateTime.Today, TimeSpan.FromMilliseconds(100))
        {
        }

        private void SyncInterval()
        {
            var now = DateTime.Now;

            if (now < FirstTickDateTime)
            {
                InternalTimer.Interval = (FirstTickDateTime - now).TotalMilliseconds;
            }
            else
            {
                var timePassed = now - FirstTickDateTime;
                double totalTicks = timePassed.TotalMilliseconds/Interval.TotalMilliseconds;
                double msUntilNextTick = (1 - totalTicks.Fraction())*Interval.TotalMilliseconds;
                InternalTimer.Interval = msUntilNextTick;
            }
        }

        protected override void TimerTick()
        {
            base.TimerTick();
            SyncInterval();
        }
    }
}