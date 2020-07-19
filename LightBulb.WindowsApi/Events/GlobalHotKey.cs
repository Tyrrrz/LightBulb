using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace LightBulb.WindowsApi.Events
{
    internal partial class GlobalHotKey : IDisposable
    {
        private readonly object _lock = new object();

        private DateTimeOffset _lastTriggerTimestamp = DateTimeOffset.MinValue;

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

        private void SpongeWindowOnMessageReceived(object? sender, Message m)
        {
            // Only messages related to this hotkey triggering
            if (m.Msg != 0x0312 || m.WParam.ToInt32() != Id)
                return;

            // Throttling
            lock (_lock)
            {
                if ((DateTimeOffset.Now - _lastTriggerTimestamp).Duration() < ThrottleInterval)
                    return;

                _lastTriggerTimestamp = DateTimeOffset.Now;
            }

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

    internal partial class GlobalHotKey
    {
        private static readonly TimeSpan ThrottleInterval = TimeSpan.FromSeconds(0.2);

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