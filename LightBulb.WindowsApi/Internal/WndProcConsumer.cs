using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace LightBulb.WindowsApi.Internal
{
    internal static partial class WndProc
    {
        private sealed class WndProcConsumer : NativeWindow
        {
            public event EventHandler<Message>? MessageReceived;

            public WndProcConsumer()
            {
                CreateHandle(new CreateParams
                {
                    Caption = "LightBulb WndProc Consumer"
                });
            }

            protected override void WndProc(ref Message m)
            {
                Debug.WriteLine($"Handling wndproc message for event {m.Msg}.");
                MessageReceived?.Invoke(this, m);
                base.WndProc(ref m);
            }
        }
    }

    internal static partial class WndProc
    {
        private static WndProcConsumer DefaultConsumer { get; } = new WndProcConsumer();

        public static IntPtr Handle => DefaultConsumer.Handle;

        // List of event IDs: https://wiki.winehq.org/List_Of_Windows_Messages
        public static IDisposable Listen(int messageId, Action<Message> callback)
        {
            void MessageReceivedHandler(object? sender, Message message)
            {
                if (message.Msg == messageId)
                    callback(message);
            }

            DefaultConsumer.MessageReceived += MessageReceivedHandler;
            Debug.WriteLine($"Added wndproc listener for event {messageId}.");

            return Disposable.Create(() =>
            {
                DefaultConsumer.MessageReceived -= MessageReceivedHandler;
                Debug.WriteLine($"Removed wndproc listener for event {messageId}.");
            });
        }
    }
}