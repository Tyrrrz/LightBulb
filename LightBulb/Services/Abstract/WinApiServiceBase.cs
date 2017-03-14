#define IgnoreWinAPIErrors

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace LightBulb.Services.Abstract
{
    /// <summary>
    /// Implements basic functionality for interacting with Windows API
    /// </summary>
    public abstract class WinApiServiceBase : IDisposable
    {
        /// <summary>
        /// Event arguments associated with a windows procedure
        /// </summary>
        protected sealed class WndProcEventArgs : EventArgs
        {
            public Message Message { get; }

            public WndProcEventArgs(Message message)
            {
                Message = message;
            }
        }

        /// <summary>
        /// Sponge window absorbs messages and lets other services use them
        /// </summary>
        protected sealed class SpongeWindow : NativeWindow
        {
            public event EventHandler<WndProcEventArgs> LocalWndProced;

            public SpongeWindow()
            {
                CreateHandle(new CreateParams());
            }

            protected override void WndProc(ref Message m)
            {
                LocalWndProced?.Invoke(this, new WndProcEventArgs(m));
                base.WndProc(ref m);
            }
        }

        private static readonly SpongeWindow Sponge;

        /// <summary>
        /// Handle for the WndProc sponge window.
        /// Use this to register for WndProc messages.
        /// </summary>
        protected static IntPtr SpongeHandle => Sponge.Handle;

        /// <summary>
        /// Triggers when there's a new Window message to be processed
        /// </summary>
        protected static event EventHandler<WndProcEventArgs> WndProced
        {
            add { Sponge.LocalWndProced += value; }
            remove { Sponge.LocalWndProced -= value; }
        }

        static WinApiServiceBase()
        {
            Sponge = new SpongeWindow();
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
            _hookHandlerDic = new Dictionary<IntPtr, WinEventHandler>();
        }

        ~WinApiServiceBase()
        {
            Dispose(false);
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
                Debug.WriteLine("Could not unregister WinEventHook", GetType().Name);
            }
            _hookHandlerDic.Remove(handle);
        }

        /// <summary>
        /// Unregister all windows event hooks
        /// </summary>
        protected void UnregisterAllWinEvents()
        {
            foreach (var hook in _hookHandlerDic)
            {
                if (!UnhookWinEventInternal(hook.Key))
                {
                    CheckLogWin32Error();
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
