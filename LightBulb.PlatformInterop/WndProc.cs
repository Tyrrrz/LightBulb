using System;
using System.Diagnostics;
using LightBulb.PlatformInterop.Utils;

namespace LightBulb.PlatformInterop;

internal static partial class WndProc
{
    private sealed class WndProcConsumer : NativeWindow
    {
        public event EventHandler<Message>? MessageReceived;

        public WndProcConsumer()
        {
            CreateHandle(new CreateParams { Caption = "LightBulb WndProc Consumer" });
        }

        protected override void WndProc(ref Message message)
        {
            Debug.WriteLine($"Handling wndproc message (ID: {message.Msg}).");
            MessageReceived?.Invoke(this, message);
            base.WndProc(ref message);
        }
    }
}

internal static partial class WndProc
{
    private static WndProcConsumer DefaultConsumer { get; } = new();

    public static nint Handle => DefaultConsumer.Handle;

    public static IDisposable Listen(int messageId, Action<Message> callback)
    {
        void MessageReceivedHandler(object? sender, Message message)
        {
            if (message.Msg == messageId)
                callback(message);
        }

        DefaultConsumer.MessageReceived += MessageReceivedHandler;
        Debug.WriteLine($"Added wndproc listener (message ID: {messageId}).");

        return Disposable.Create(() =>
        {
            DefaultConsumer.MessageReceived -= MessageReceivedHandler;
            Debug.WriteLine($"Removed wndproc listener (message ID: {messageId}).");
        });
    }
}

internal static partial class WndProc
{
    // List of message IDs: https://wiki.winehq.org/List_Of_Windows_Messages
    public static class Ids
    {
        public static int PowerSettingMessage => 536;
        public static int GlobalHotkeyMessage => 786;
    }
}
