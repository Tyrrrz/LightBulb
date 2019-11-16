using System;
using System.Windows.Forms;

namespace LightBulb.WindowsApi.Internal
{
    internal sealed class SpongeWindow : NativeWindow
    {
        public event EventHandler<Message>? MessageReceived;

        public SpongeWindow()
        {
            CreateHandle(new CreateParams());
        }

        protected override void WndProc(ref Message m)
        {
            MessageReceived?.Invoke(this, m);
            base.WndProc(ref m);
        }
    }
}