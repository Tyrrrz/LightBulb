using System;
using System.Windows.Forms;

namespace LightBulb.WindowsApi.Events
{
    internal sealed partial class SpongeWindow : NativeWindow
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

    internal sealed partial class SpongeWindow
    {
        public static SpongeWindow Instance { get; } = new SpongeWindow();
    }
}