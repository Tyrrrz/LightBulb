using System;

namespace LightBulb.WindowsApi.Internal
{
    internal partial class WinEvent : IDisposable
    {
        private readonly IntPtr _handle;
        private readonly NativeMethods.WinEventHandler _handler;

        public WinEvent(IntPtr handle, NativeMethods.WinEventHandler handler)
        {
            _handle = handle;
            _handler = handler;
        }

        ~WinEvent()
        {
            Dispose();
        }

        public void Dispose()
        {
            NativeMethods.UnhookWinEvent(_handle);
            GC.SuppressFinalize(this);
        }
    }

    internal partial class WinEvent
    {
        public static WinEvent Register(uint eventId, uint processId, uint threadId,
            NativeMethods.WinEventHandler handler)
        {
            var handle = NativeMethods.SetWinEventHook(eventId, eventId, IntPtr.Zero, handler, processId, threadId, 0);
            return new WinEvent(handle, handler);
        }

        public static WinEvent Register(uint eventId, uint threadId, NativeMethods.WinEventHandler handler) =>
            Register(eventId, 0, threadId, handler);

        public static WinEvent Register(uint eventId, NativeMethods.WinEventHandler handler) =>
            Register(eventId, 0, handler);
    }
}