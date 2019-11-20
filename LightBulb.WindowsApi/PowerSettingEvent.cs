using System;
using System.Diagnostics;
using System.Windows.Forms;
using LightBulb.WindowsApi.Internal;

namespace LightBulb.WindowsApi
{
    public partial class PowerSettingEvent
    {
        public IntPtr Handle { get; }

        public Guid Id { get; }

        public Action Handler { get; }

        public PowerSettingEvent(IntPtr handle, Guid id, Action handler)
        {
            Handle = handle;
            Id = id;
            Handler = handler;

            // Wire up wnd proc events
            SpongeWindow.Instance.MessageReceived += SpongeWindowOnMessageReceived;
        }

        private void SpongeWindowOnMessageReceived(object? sender, Message m)
        {
            // Only PowerSettingChange messages
            if (m.Msg != 0x218 || m.WParam.ToInt32() != 0x8013)
                return;

            // Trigger handler
            Handler();
        }

        ~PowerSettingEvent()
        {
            Dispose();
        }

        public void Dispose()
        {
            // Unwire wnd proc events
            SpongeWindow.Instance.MessageReceived -= SpongeWindowOnMessageReceived;

            if (!NativeMethods.UnregisterPowerSettingNotification(Handle))
                Debug.WriteLine("Could not dispose power setting event.");

            GC.SuppressFinalize(this);
        }
    }

    public partial class PowerSettingEvent
    {
        public static PowerSettingEvent? Register(Guid id, Action handler)
        {
            var handle = NativeMethods.RegisterPowerSettingNotification(SpongeWindow.Instance.Handle, ref id, 0);
            return handle != IntPtr.Zero ? new PowerSettingEvent(handle, id, handler) : null;
        }
    }
}