using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using LightBulb.PlatformInterop.Utils;

namespace LightBulb.PlatformInterop.Internal;

internal partial class WndProcSponge(
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
        // Capture the calling thread's SynchronizationContext so that callbacks are
        // dispatched back to it (typically Avalonia's UI thread) rather than running
        // on the WndProcSponge's dedicated background thread.
        var syncContext = SynchronizationContext.Current;

        void OnMessageBroadcasted(object? _, WndProcMessage message)
        {
            if (message.Id != messageId)
                return;

            if (syncContext is { } ctx)
                ctx.Post(_ => callback(message), null);
            else
                callback(message);
        }

        broadcaster.MessageBroadcasted += OnMessageBroadcasted;

        return Disposable.Create(() => broadcaster.MessageBroadcasted -= OnMessageBroadcasted);
    }

    public void Dispose()
    {
        // Post WM_CLOSE to the window's dedicated thread. DefWindowProc will call
        // DestroyWindow, which sends WM_DESTROY; the WndProc handler then calls
        // PostQuitMessage(0) so GetMessage returns 0 and the message loop exits.
        // UnregisterClass is handled by the background thread after the loop exits.
        NativeMethods.PostMessage(
            windowHandle,
            0x0010 /* WM_CLOSE */
            ,
            0,
            0
        );
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
                // When the window is destroyed (e.g. via WM_CLOSE -> DefWindowProc ->
                // DestroyWindow), post WM_QUIT so the dedicated message loop exits.
                if (
                    message == 0x0002 /* WM_DESTROY */
                )
                    NativeMethods.PostQuitMessage(0);

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

        WndProcSponge? result = null;
        using var ready = new ManualResetEventSlim(false);

        // Create the window and run its message loop on a dedicated background STA thread.
        // This ensures WM_POWERBROADCAST (from RegisterPowerSettingNotification) and other
        // posted messages are dispatched promptly even when Avalonia's UI thread is idle.
        var thread = new Thread(() =>
        {
            try
            {
                var classHandle = NativeMethods.RegisterClassEx(ref classInfo);
                if (classHandle == 0)
                {
                    Debug.WriteLine(
                        "Failed to register window class. "
                            + $"Error {Marshal.GetLastWin32Error()}."
                    );

                    return;
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
                    Debug.WriteLine(
                        "Failed to create window. " + $"Error {Marshal.GetLastWin32Error()}."
                    );
                    NativeMethods.UnregisterClass(ClassName, NativeModule.CurrentHandle);
                    return;
                }

                result = new WndProcSponge(windowHandle, wndProc, broadcaster);
            }
            finally
            {
                // Always signal readiness so the calling thread doesn't block indefinitely.
                ready.Set();
            }

            // Run a dedicated message loop so posted messages (including WM_POWERBROADCAST
            // from RegisterPowerSettingNotification) are always dispatched, even when
            // the application's UI thread is idle and not pumping its own message queue.
            while (NativeMethods.GetMessage(out var msg, IntPtr.Zero, 0, 0) > 0)
            {
                NativeMethods.TranslateMessage(ref msg);
                NativeMethods.DispatchMessage(ref msg);
            }

            // Window was destroyed via WM_CLOSE -> WM_DESTROY -> PostQuitMessage.
            NativeMethods.UnregisterClass(ClassName, NativeModule.CurrentHandle);
        })
        {
            IsBackground = true,
            Name = "WndProcSponge",
        };

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        ready.Wait();
        return result;
    }
}
