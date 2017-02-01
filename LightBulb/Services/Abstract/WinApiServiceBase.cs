#define IgnoreWinAPIErrors

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LightBulb.Services.Abstract
{
    public abstract class WinApiServiceBase : IDisposable
    {
        /// <summary>
        /// Sponge window absorbs messages and lets other services use them
        /// </summary>
        private sealed class SpongeWindow : NativeWindow
        {
            private readonly Action<Message> _messageHandler;

            public SpongeWindow(Action<Message> messageHandler)
            {
                _messageHandler = messageHandler;
                CreateHandle(new CreateParams());
            }

            protected override void WndProc(ref Message m)
            {
                _messageHandler(m);
                base.WndProc(ref m);
            }
        }

        private static readonly SpongeWindow Sponge;
        protected static readonly IntPtr SpongeHandle;

        private static event EventHandler<Message> WndProced;

        static WinApiServiceBase()
        {
            Sponge = new SpongeWindow(m => WndProced?.Invoke(Sponge, m));
            SpongeHandle = Sponge.Handle;
        }

        protected delegate void WinEventHandler(
            IntPtr hWinEventHook, uint eventType, IntPtr hWnd,
            int idObject, int idChild, uint dwEventThread,
            uint dwmsEventTime);

        #region WinAPI
        [DllImport("user32.dll", EntryPoint = "SetWinEventHook", SetLastError = true)]
        private static extern IntPtr SetWinEventHookInternal(
            uint eventMin, uint eventMax,
            IntPtr hmodWinEventProc, WinEventHandler lpfnWinEventProc,
            uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll", EntryPoint = "UnhookWinEvent", SetLastError = true)]
        private static extern bool UnhookWinEventInternal(IntPtr hWinEventHook);
        #endregion

        private readonly Dictionary<IntPtr, WinEventHandler> _hookHandlerDic;

        protected WinApiServiceBase()
        {
            WndProced += LocalWndProced;
            _hookHandlerDic = new Dictionary<IntPtr, WinEventHandler>();
        }

        private void LocalWndProced(object sender, Message message)
        {
            WndProc(message);
        }

        /// <summary>
        /// Get the last WinApi error wrapped in <see cref="Win32Exception"/>
        /// </summary>
        protected Win32Exception GetLastError()
        {
            int errCode = Marshal.GetLastWin32Error();
            if (errCode == 0) return null;
            return new Win32Exception(errCode);
        }

        /// <summary>
        /// Check if last action raised an error and log it if so
        /// </summary>
        protected void CheckLogWin32Error()
        {
#if !IgnoreWinAPIErrors
            var ex = GetLastError();
            if (ex != null) Debug.WriteLine($"Win32 error: {ex.Message} ({ex.NativeErrorCode})", GetType().Name);
#endif
        }

        /// <summary>
        /// Override to process windows messages
        /// </summary>
        protected virtual void WndProc(Message message)
        { }

        /// <summary>
        /// Register a windows event hook
        /// </summary>
        protected IntPtr RegisterWinEvent(
            uint eventId, WinEventHandler handler,
            uint processId = 0, uint threadId = 0, uint flags = 0)
        {
            var handle = SetWinEventHookInternal(eventId, eventId, IntPtr.Zero, handler, processId, threadId, flags);
            if (handle == IntPtr.Zero)
            {
                CheckLogWin32Error();
                Debug.WriteLine($"Could not register WinEventHook for {eventId}", GetType().Name);
                return IntPtr.Zero;
            }

            _hookHandlerDic.Add(handle, handler);
            return handle;
        }

        /// <summary>
        /// Unregister a windows event hook
        /// </summary>
        protected void UnregisterWinEvent(IntPtr handle)
        {
            if (!UnhookWinEventInternal(handle))
            {
                CheckLogWin32Error();
            }
            _hookHandlerDic.Remove(handle);
        }

        public virtual void Dispose()
        {
            WndProced -= LocalWndProced;
            foreach (var hook in _hookHandlerDic)
                UnregisterWinEvent(hook.Key);
        }
    }
}
