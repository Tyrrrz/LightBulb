using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LightBulb.WindowsApi.Internal;

namespace LightBulb.WindowsApi
{
    public class PowerSettingManager : IDisposable
    {
        private readonly Dictionary<Guid, Tuple<Action<byte>, IntPtr>> _settingHandlersMap = new Dictionary<Guid, Tuple<Action<byte>, IntPtr>>();
        private readonly WndProcSpongeWindow _wndProcSponge;

        public PowerSettingManager()
        {
            _wndProcSponge = new WndProcSpongeWindow(m =>
            {
                // Only PowerSettingChange messages
                if (m.Msg != 0x218 || m.WParam.ToInt32() != 0x8013)
                    return;

                var eventId = m.WParam.ToInt32();
                var powerSetting = Marshal.PtrToStructure<NativeMethods.PowerBroadcastSetting>(m.LParam);

                _settingHandlersMap.GetValueOrDefault(powerSetting.PowerSetting)?.Item1.Invoke(powerSetting.Data);
            });
        }

        ~PowerSettingManager()
        {
            Dispose();
        }

        public void RegisterPowerEvent(Guid settingGuid, Action<byte> handler)
        {
            IntPtr handle = NativeMethods.RegisterPowerSettingNotification(_wndProcSponge.Handle, ref settingGuid, 0);
            var value = new Tuple<Action<byte>, IntPtr>(handler, handle);

            _settingHandlersMap.Add(settingGuid, value);
        }

        public void UnregisterEvents()
        {
            _settingHandlersMap.Clear();

            foreach(var item in _settingHandlersMap.Values) 
                NativeMethods.UnregisterPowerSettingNotification(item.Item2);
        }

        public void Dispose()
        {
            UnregisterEvents();
             GC.SuppressFinalize(this);
        }
    }
}
