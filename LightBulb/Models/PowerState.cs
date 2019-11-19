using System;

namespace LightBulb.WindowsApi
{
    public abstract class PowerState
    {
        public virtual Guid Guid { get; }
        protected virtual int State { get; set; }

        public PowerState(int state) { State = state; }
    }

    public class DisplayState : PowerState
    {
        public override Guid Guid { get { return new Guid("6FE69556-704A-47A0-8F24-C28D936FDA47"); } }

        public enum States { Off, On, Dimmed }
        public States GetState() { return (States)State; }

        public DisplayState(int state) : base(state) { }
    }
}
