using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using LightBulb.WindowsApi.Internal;


namespace LightBulb.WindowsApi
{
    public partial class PowerBroadcast
    {
        public Action<byte> Handler { get; }
        public Guid Id { get; }

        readonly IntPtr _handle;

        public PowerBroadcast(IntPtr handle, Action<byte> handler, Guid id)
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

            Handler(powerSetting.Data);
        }

        ~PowerBroadcast()
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

    public partial class PowerBroadcast
    {
        private static readonly SpongeWindow SpongeWindow = new SpongeWindow();

        public static PowerBroadcast? Register(Action<byte> handler, Guid id)
        {
            IntPtr handle = NativeMethods.RegisterPowerSettingNotification(SpongeWindow.Handle, ref id, 0);

            return handle != null ? new PowerBroadcast(handle, handler, id) : null;
        }
    }
}
