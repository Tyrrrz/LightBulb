using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using LightBulb.WindowsApi.Internal;

namespace LightBulb.WindowsApi
{
    public partial class PowerBroadcast
    {
        public Action Handler { get; }
        public Guid Id { get; }

        readonly IntPtr handle;

        public PowerBroadcast(Action handler, Guid id)
        {
            Handler = handler;
            Id = id;
            SpongeWindow.MessageReceived += SpongeWindowOnMessageReceived;
            // Register for notification and store the handle to deregister
            handle = NativeMethods.RegisterPowerSettingNotification(SpongeWindow.Handle, ref id, 0);
        }

        private void SpongeWindowOnMessageReceived(object? sender, Message m)
        {
            // Only PowerSettingChange messages
            if (m.Msg != 0x218 || m.WParam.ToInt32() != 0x8013)
                return;

            var powerSetting = Marshal.PtrToStructure<PowerBroadcastSetting>(m.LParam);

            Handler();
        }

        ~PowerBroadcast()
        {
            Dispose();
        }

        public void Dispose()
        {
            SpongeWindow.Instance.MessageReceived -= SpongeWindowOnMessageReceived;

            if (!NativeMethods.UnregisterPowerSettingNotification(handle))
                Debug.WriteLine("Could not power broadcast.");

            GC.SuppressFinalize(this);
        }
    }

    public partial class PowerBroadcast
    {
        private static readonly SpongeWindow SpongeWindow = new SpongeWindow();
    }
}
