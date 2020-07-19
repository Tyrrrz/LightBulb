using System;
using System.Runtime.InteropServices;

namespace LightBulb.WindowsApi.Events
{
    internal static class NativeMethods
    {
        private const string User = "user32.dll";

        [DllImport(User, SetLastError = true)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport(User, SetLastError = true)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport(User, SetLastError = true)]
        public static extern IntPtr RegisterPowerSettingNotification(IntPtr hRecipient, ref Guid powerSettingGuid, int flags);

        [DllImport(User, SetLastError = true)]
        public static extern bool UnregisterPowerSettingNotification(IntPtr handle);
    }
}