using System;
using System.Windows.Forms;
using LightBulb.WindowsApi.Internal;

namespace LightBulb.WindowsApi
{
    public partial class NativeHotKey : IDisposable
    {
        public int Id { get; }

        public int VirtualKey { get; }

        public int Modifiers { get; }

        public Action Handler { get; }

        public NativeHotKey(int id, int virtualKey, int modifiers, Action handler)
        {
            Id = id;
            VirtualKey = virtualKey;
            Modifiers = modifiers;
            Handler = handler;

            // Wire up wnd proc events
            SpongeWindow.MessageReceived += SpongeWindowOnMessageReceived;
        }

        ~NativeHotKey()
        {
            Dispose();
        }

        private void SpongeWindowOnMessageReceived(object? sender, Message e)
        {
            // Only hotkey-related messages
            if (e.Msg != 0x0312)
                return;

            // Only this hotkey
            if (e.WParam.ToInt32() != Id)
                return;

            // Trigger handler
            Handler();
        }

        public void Dispose()
        {
            NativeMethods.UnregisterHotKey(SpongeWindow.Handle, Id);
            SpongeWindow.MessageReceived -= SpongeWindowOnMessageReceived;
            GC.SuppressFinalize(this);
        }
    }

    public partial class NativeHotKey
    {
        private static int _lastHotKeyId;

        public static NativeHotKey Register(int virtualKey, int modifiers, Action handler)
        {
            var id = _lastHotKeyId++;
            NativeMethods.RegisterHotKey(SpongeWindow.Handle, id, modifiers, virtualKey);

            return new NativeHotKey(id, virtualKey, modifiers, handler);
        }
    }

    public partial class NativeHotKey
    {
        private static readonly SpongeWindow SpongeWindow = new SpongeWindow();
    }
}