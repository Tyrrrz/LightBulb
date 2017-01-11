using System;
using NegativeLayer.Extensions;

namespace LightBulb.Services
{
    /// <summary>
    /// Timer that synchronizes its interval with the system clock
    /// </summary>
    public class SyncedTimer : Timer
    {
        /// <summary>
        /// The date/time of the first time this timer should fire an event
        /// </summary>
        public DateTime FirstTickDateTime { get; set; }

        public SyncedTimer(DateTime firstTickDateTime, TimeSpan interval)
            : base(interval)
        {
            FirstTickDateTime = firstTickDateTime;
        }

        public SyncedTimer()
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
                double periods = timePassed.TotalMilliseconds/Interval.TotalMilliseconds;
                double msUntilNextPeriod = (1 - periods.Fraction())*Interval.TotalMilliseconds;
                InternalTimer.Interval = msUntilNextPeriod;
            }
        }

        protected override void Start()
        {
            SyncInterval();
            InternalTimer.Start();
        }

        protected override void TimerTick()
        {
            base.TimerTick();
            SyncInterval();
        }
    }
}