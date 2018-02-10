using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LightBulb.Internal
{
    /// <summary>
    /// Handler for Windows hook events
    /// </summary>
    internal delegate void WinEventHandler(
        IntPtr hWinEventHook, uint eventType, IntPtr hWnd,
        int idObject, int idChild, uint dwEventThread,
        uint dwmsEventTime);

    internal class HookManager : IDisposable
    {
        private readonly Dictionary<IntPtr, WinEventHandler> _hookHandlerDic;

        public HookManager()
        {
            _hookHandlerDic = new Dictionary<IntPtr, WinEventHandler>();
        }

        ~HookManager()
        {
            Dispose(false);
        }

        /// <summary>
        /// Register a windows event hook
        /// </summary>
        public IntPtr RegisterWinEvent(
            uint eventId, WinEventHandler handler,
            uint processId = 0, uint threadId = 0, uint flags = 0)
        {
            var handle = NativeMethods.SetWinEventHook(eventId, eventId, IntPtr.Zero, handler, processId, threadId, flags);
            if (handle == IntPtr.Zero)
            {
                Debug.WriteLine($"Could not register WinEventHook for {eventId}", GetType().Name);
                return IntPtr.Zero;
            }

            _hookHandlerDic.Add(handle, handler);
            return handle;
        }

        /// <summary>
        /// Unregister a windows event hook
        /// </summary>
        public void UnregisterWinEvent(IntPtr handle)
        {
            if (!NativeMethods.UnhookWinEvent(handle))
            {
                Debug.WriteLine("Could not unregister WinEventHook", GetType().Name);
            }

            _hookHandlerDic.Remove(handle);
        }

        /// <summary>
        /// Unregister all windows event hooks
        /// </summary>
        public void UnregisterAllWinEvents()
        {
            foreach (var hook in _hookHandlerDic)
            {
                if (!NativeMethods.UnhookWinEvent(hook.Key))
                {
                    Debug.WriteLine("Could not unregister WinEventHook", GetType().Name);
                }
            }

            _hookHandlerDic.Clear();
        }

        protected virtual void Dispose(bool disposing)
        {
            UnregisterAllWinEvents();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}