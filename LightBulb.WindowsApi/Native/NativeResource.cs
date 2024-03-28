using System;
using System.Diagnostics.CodeAnalysis;

namespace LightBulb.WindowsApi.Native;

public abstract class NativeResource(nint handle) : IDisposable
{
    public nint Handle { get; } = handle;

    [ExcludeFromCodeCoverage]
    ~NativeResource() => Dispose(false);

    protected abstract void Dispose(bool disposing);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
