using System;
using System.Diagnostics.CodeAnalysis;

namespace LightBulb.WindowsApi;

public abstract class NativeResource : IDisposable
{
    public nint Handle { get; }

    protected NativeResource(nint handle) => Handle = handle;

    [ExcludeFromCodeCoverage]
    ~NativeResource() => Dispose(false);

    protected abstract void Dispose(bool disposing);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
