using System;
using System.Diagnostics;
using LightBulb.WindowsApi.Types;
using LightBulb.WindowsApi.Utils.Extensions;

namespace LightBulb.WindowsApi;

public partial class PowerSettingNotification : NativeResource
{
    private readonly IDisposable _wndProcRegistration;

    private PowerSettingNotification(nint handle, Guid powerSettingId, Action callback)
        : base(handle)
    {
        _wndProcRegistration = WndProc.Listen(WndProc.Ids.PowerSettingMessage, m =>
        {
            // Filter out other power events
            if (m.GetLParam<PowerBroadcastSetting>().PowerSettingId != powerSettingId)
                return;

            callback();
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _wndProcRegistration.Dispose();

        if (!NativeMethods.UnregisterPowerSettingNotification(Handle))
            Debug.WriteLine($"Failed to dispose power setting notification #{Handle}.");
    }
}

public partial class PowerSettingNotification
{
    public static PowerSettingNotification? TryRegister(Guid powerSettingId, Action callback)
    {
        var handle = NativeMethods.RegisterPowerSettingNotification(WndProc.Handle, powerSettingId, 0);
        if (handle == 0)
        {
            Debug.WriteLine($"Failed to register power setting notification (ID: {powerSettingId}).");
            return null;
        }

        return new PowerSettingNotification(handle, powerSettingId, callback);
    }
}

public partial class PowerSettingNotification
{
    public static class Ids
    {
        public static Guid ConsoleDisplayStateChanged { get; } = Guid.Parse("6FE69556-704A-47A0-8F24-C28D936FDA47");
        public static Guid MonitorPowerStateChanged { get; } = Guid.Parse("02731015-4510-4526-99E6-E5A17EBD1AEA");
        public static Guid PowerSavingStatusChanged { get; } = Guid.Parse("E00958C0-C213-4ACE-AC77-FECCED2EEEA5");
        public static Guid SessionDisplayStatusChanged { get; } = Guid.Parse("2B84C20E-AD23-4ddf-93DB-05FFBD7EFCA5");
        public static Guid AwayModeChanged { get; } = Guid.Parse("98A7F580-01F7-48AA-9C0F-44352C29E5C0");
    }
}