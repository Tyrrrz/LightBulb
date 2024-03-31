using System;
using System.Diagnostics.CodeAnalysis;

namespace LightBulb.PlatformInterop.Internal;

public abstract class NativeResource<T>(T handle) : IDisposable
{
    public T Handle { get; } = handle;

    [ExcludeFromCodeCoverage]
    ~NativeResource() => Dispose(false);

    protected abstract void Dispose(bool disposing);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

public abstract class NativeResource(nint handle) : NativeResource<nint>(handle);