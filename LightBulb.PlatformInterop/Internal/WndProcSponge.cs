using System;
using System.Diagnostics;
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
            Debug.WriteLine($"Failed to destroy window #{windowHandle}.");

        if (!NativeMethods.UnregisterClass(ClassName, NativeModule.CurrentHandle))
            Debug.WriteLine($"Failed to unregister window class #{classHandle}.");
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
            Debug.WriteLine("Failed to register window class.");
            return null;
        }

        var windowHandle = NativeMethods.CreateWindowEx(
            0,
            ClassName,
            ClassName,
            0,
            0,
            0,
            0,
            0,
            -3, // HWND_MESSAGE
            0,
            0,
            0
        );

        if (windowHandle == 0)
        {
            Debug.WriteLine("Failed to create window.");
            NativeMethods.UnregisterClass(ClassName, NativeModule.CurrentHandle);
            return null;
        }

        return new WndProcSponge(classHandle, windowHandle, wndProc, broadcaster);
    }
}
