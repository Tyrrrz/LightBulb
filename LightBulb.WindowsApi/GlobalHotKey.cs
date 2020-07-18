using System;
using System.Diagnostics;
using System.Windows.Forms;
using LightBulb.WindowsApi.Internal;

namespace LightBulb.WindowsApi
{
    public partial class GlobalHotKey : IDisposable
    {
        public int Id { get; }

        public int VirtualKey { get; }

        public int Modifiers { get; }

        public Action Handler { get; }

        public GlobalHotKey(int id, int virtualKey, int modifiers, Action handler)
        {
            Id = id;
            VirtualKey = virtualKey;
            Modifiers = modifiers;
            Handler = handler;

            SpongeWindow.Instance.MessageReceived += SpongeWindowOnMessageReceived;
        }

        ~GlobalHotKey() => Dispose();

        private void SpongeWindowOnMessageReceived(object? sender, Message e)
        {
            // Only messages related to this hotkey triggering
            if (e.Msg != 0x0312 || e.WParam.ToInt32() != Id)
                return;

            Handler();
        }

        public void Dispose()
        {
            SpongeWindow.Instance.MessageReceived -= SpongeWindowOnMessageReceived;

            if (!NativeMethods.UnregisterHotKey(SpongeWindow.Instance.Handle, Id))
                Debug.WriteLine("Could not dispose global hotkey.");

            GC.SuppressFinalize(this);
        }
    }

    public partial class GlobalHotKey
    {
        private static int _lastHotKeyId;

        public static GlobalHotKey? TryRegister(int virtualKey, int modifiers, Action handler)
        {
            var id = _lastHotKeyId++;
            return NativeMethods.RegisterHotKey(SpongeWindow.Instance.Handle, id, modifiers, virtualKey)
                ? new GlobalHotKey(id, virtualKey, modifiers, handler)
                : null;
        }
    }
}