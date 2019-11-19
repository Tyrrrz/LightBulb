using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using LightBulb.WindowsApi.Internal;


namespace LightBulb.WindowsApi
{
    public partial class PowerSettingEvent
    {
        public Action Handler { get; }
        public Guid Id { get; }

        readonly IntPtr _handle;

        public PowerSettingEvent(IntPtr handle, Action handler, Guid id)
        {
            Handler = handler;
            Id = id;
            SpongeWindow.MessageReceived += SpongeWindowOnMessageReceived;
            _handle = handle;
        }

        private void SpongeWindowOnMessageReceived(object? sender, Message m)
        {
            // Only PowerSettingChange messages
            if (m.Msg != 0x218 || m.WParam.ToInt32() != 0x8013)
                return;

            var powerSetting = Marshal.PtrToStructure<PowerBroadcastSetting>(m.LParam);

            Handler();
        }

        ~PowerSettingEvent()
        {
            Dispose();
        }

        public void Dispose()
        {
            SpongeWindow.Instance.MessageReceived -= SpongeWindowOnMessageReceived;

            if (!NativeMethods.UnregisterPowerSettingNotification(_handle))
                Debug.WriteLine("Could not power broadcast.");

            GC.SuppressFinalize(this);
        }
    }

    public partial class PowerSettingEvent
    {
        private static readonly SpongeWindow SpongeWindow = new SpongeWindow();

        public static PowerSettingEvent? Register(Guid id, Action handler)
        {
            IntPtr handle = NativeMethods.RegisterPowerSettingNotification(SpongeWindow.Handle, ref id, 0);

            return handle != null ? new PowerSettingEvent(handle, handler, id) : null;
        }
    }
}
