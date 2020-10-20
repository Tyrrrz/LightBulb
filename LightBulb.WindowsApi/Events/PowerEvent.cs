using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace LightBulb.WindowsApi.Events
{
    internal partial class PowerEvent : IDisposable
    {
        public IntPtr Handle { get; }

        public Guid Id { get; }

        public Action Handler { get; }

        public PowerEvent(IntPtr handle, Guid id, Action handler)
        {
            Handle = handle;
            Id = id;
            Handler = handler;

            SpongeWindow.Instance.MessageReceived += SpongeWindowOnMessageReceived;
        }

        ~PowerEvent() => Dispose();

        private void SpongeWindowOnMessageReceived(object? sender, Message m)
        {
            // Only messages related to this event triggering
            if (m.Msg != 0x218 || m.WParam.ToInt32() != 0x8013)
                return;

            Handler();
        }

        public void Dispose()
        {
            SpongeWindow.Instance.MessageReceived -= SpongeWindowOnMessageReceived;

            if (!NativeMethods.UnregisterPowerSettingNotification(Handle))
                Debug.WriteLine("Could not dispose power setting event.");

            GC.SuppressFinalize(this);
        }
    }

    internal partial class PowerEvent
    {
        public static Guid ConsoleDisplayStateId { get; } = Guid.Parse("6FE69556-704A-47A0-8F24-C28D936FDA47");

        public static Guid MonitorPowerOnId { get; } = Guid.Parse("02731015-4510-4526-99E6-E5A17EBD1AEA");

        public static Guid PowerSavingStatusId { get; } = Guid.Parse("E00958C0-C213-4ACE-AC77-FECCED2EEEA5");

        public static Guid SessionDisplayStatusId { get; } = Guid.Parse("2B84C20E-AD23-4ddf-93DB-05FFBD7EFCA5");

        public static Guid AwayModeId { get; } = Guid.Parse("98A7F580-01F7-48AA-9C0F-44352C29E5C0");

        public static PowerEvent? TryRegister(Guid id, Action handler)
        {
            var handle = NativeMethods.RegisterPowerSettingNotification(SpongeWindow.Instance.Handle, ref id, 0);
            return handle != IntPtr.Zero
                ? new PowerEvent(handle, id, handler)
                : null;
        }
    }
}