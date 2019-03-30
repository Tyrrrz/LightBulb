using System;
using System.Windows.Forms;

namespace LightBulb.WindowsApi.Internal
{
    internal sealed class WndProcSpongeWindow : NativeWindow
    {
        private readonly Action<Message> _handler;

        public WndProcSpongeWindow(Action<Message> handler)
        {
            _handler = handler;

            CreateHandle(new CreateParams());
        }

        protected override void WndProc(ref Message m)
        {
            _handler?.Invoke(m);
            base.WndProc(ref m);
        }
    }
}