using System;
using System.Diagnostics;
using System.Text;
using LightBulb.WindowsApi.Native;

namespace LightBulb.WindowsApi;

public partial class SystemProcess : IDisposable
{
    private readonly IntPtr _handle;

    private SystemProcess(IntPtr handle) => _handle = handle;

    ~SystemProcess() => Dispose();

    public string? TryGetExecutableFilePath()
    {
        var buffer = new StringBuilder(1024);
        var bufferSize = (uint) buffer.Capacity + 1;

        return NativeMethods.QueryFullProcessImageName(_handle, 0, buffer, ref bufferSize)
            ? buffer.ToString()
            : null;
    }

    public void Dispose()
    {
        if (!NativeMethods.CloseHandle(_handle))
            Debug.WriteLine($"Failed to dispose process (handle: {_handle}).");

        GC.SuppressFinalize(this);
    }
}

public partial class SystemProcess
{
    public static SystemProcess? TryOpen(int processId)
    {
        var handle = NativeMethods.OpenProcess(ProcessAccessFlags.QueryLimitedInformation, false, (uint) processId);

        if (handle == IntPtr.Zero)
        {
            Debug.WriteLine($"Failed to open process (ID: {processId}).");
            return null;
        }

        return new SystemProcess(handle);
    }
}