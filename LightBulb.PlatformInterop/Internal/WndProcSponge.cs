using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using LightBulb.PlatformInterop.Utils;

namespace LightBulb.PlatformInterop.Internal;

internal partial class WndProcSponge(
    nint classHandle,
    nint windowHandle,
    WndProc wndProc,
    WndProcBroadcaster broadcaster
) : IDisposable
{
    // We only need the reference to the delegate to prevent it from being garbage collected too early
    // ReSharper disable once UnusedMember.Local
    private readonly WndProc _wndProc = wndProc;

    ~WndProcSponge() => Dispose();

    public nint Handle => windowHandle;

    public IDisposable Listen(int messageId, Action<WndProcMessage> callback)
    {
        void OnMessageBroadcasted(object? _, WndProcMessage message)
        {
            if (message.Id == messageId)
                callback(message);
        }

        broadcaster.MessageBroadcasted += OnMessageBroadcasted;

        return Disposable.Create(() => broadcaster.MessageBroadcasted -= OnMessageBroadcasted);
    }

    public void Dispose()
    {
        if (!NativeMethods.DestroyWindow(windowHandle))
        {
            Debug.WriteLine(
                $"Failed to destroy window #{windowHandle}. "
                    + $"Error {Marshal.GetLastWin32Error()}."
            );
        }

        if (!NativeMethods.UnregisterClass(ClassName, NativeModule.CurrentHandle))
        {
            Debug.WriteLine(
                $"Failed to unregister window class #{classHandle}. "
                    + $"Error {Marshal.GetLastWin32Error()}."
            );
        }
    }
}

internal partial class WndProcSponge
{
    private const string ClassName = "LightBulb.WndProcSponge";

    private static readonly Lazy<WndProcSponge> DefaultLazy = new(() =>
        TryCreate() ?? throw new InvalidOperationException("Failed to create WndProc sponge.")
    );

    public static WndProcSponge Default => DefaultLazy.Value;

    public static WndProcSponge? TryCreate()
    {
        var broadcaster = new WndProcBroadcaster();

        var wndProc = new WndProc(
            (windowHandle, message, wParam, lParam) =>
            {
                broadcaster.BroadcastMessage(message, wParam, lParam);
                return NativeMethods.DefWindowProc(windowHandle, message, wParam, lParam);
            }
        );

        var classInfo = new WndClassEx
        {
            ClassName = ClassName,
            WndProc = wndProc,
            Instance = NativeModule.CurrentHandle,
        };

        var classHandle = NativeMethods.RegisterClassEx(ref classInfo);
        if (classHandle == 0)
        {
            Debug.WriteLine(
                "Failed to register window class. " + $"Error {Marshal.GetLastWin32Error()}."
            );

            return null;
        }

        var windowHandle = NativeMethods.CreateWindowEx(
            // WS_EX_TOOLWINDOW: exclude from taskbar and Alt+Tab
            // WS_EX_NOACTIVATE: don't steal focus
            0x08000080,
            ClassName,
            ClassName,
            // WS_POPUP: no title bar or border; no WS_VISIBLE so the window is hidden
            0x80000000,
            0,
            0,
            0,
            0,
            // NULL parent (not HWND_MESSAGE) so the window receives broadcast messages
            // like WM_SYSCOMMAND, which message-only windows do not receive
            0,
            0,
            0,
            0
        );

        if (windowHandle == 0)
        {
            Debug.WriteLine("Failed to create window. " + $"Error {Marshal.GetLastWin32Error()}.");
            NativeMethods.UnregisterClass(ClassName, NativeModule.CurrentHandle);
            return null;
        }

        return new WndProcSponge(classHandle, windowHandle, wndProc, broadcaster);
    }
}
