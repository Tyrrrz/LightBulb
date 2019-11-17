﻿using System;
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

            // Wire up wnd proc events
            SpongeWindow.Instance.MessageReceived += SpongeWindowOnMessageReceived;
        }

        ~GlobalHotKey()
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
            // Unwire wnd proc events
            SpongeWindow.Instance.MessageReceived -= SpongeWindowOnMessageReceived;

            // Potentially unhandled error
            NativeMethods.UnregisterHotKey(SpongeWindow.Instance.Handle, Id);

            GC.SuppressFinalize(this);
        }
    }

    public partial class GlobalHotKey
    {
        private static int _lastHotKeyId;

        public static GlobalHotKey Register(int virtualKey, int modifiers, Action handler)
        {
            var id = _lastHotKeyId++;

            // Potentially unhandled error
            NativeMethods.RegisterHotKey(SpongeWindow.Instance.Handle, id, modifiers, virtualKey);

            return new GlobalHotKey(id, virtualKey, modifiers, handler);
        }
    }
}