using System;

namespace LightBulb.Models.PowerState
{
    public abstract class PowerState
    {
        protected int _state;

        public void SetState(int state) { _state = state; }
        public int GetState() { return _state; }

        public virtual Guid Guid { get; }
    }

    public class Display : PowerState
    {
        public override Guid Guid { get { return new Guid("6FE69556-704A-47A0-8F24-C28D936FDA47"); } }

        public enum States { Off, On, Dimmed }
        public new States GetState() { return (States)_state; }
    }
}
