using System.Diagnostics;
using System.Text;
using LightBulb.PlatformInterop.Internal;

namespace LightBulb.PlatformInterop;

public partial class NativeProcess(nint handle) : NativeResource(handle)
{
    public string? TryGetExecutableFilePath()
    {
        var buffer = new StringBuilder(1024);
        var bufferSize = (uint)buffer.Capacity + 1;

        return NativeMethods.QueryFullProcessImageName(Handle, 0, buffer, ref bufferSize)
            ? buffer.ToString()
            : null;
    }

    protected override void Dispose(bool disposing)
    {
        if (!NativeMethods.CloseHandle(Handle))
            Debug.WriteLine($"Failed to dispose process #{Handle}.");
    }
}

public partial class NativeProcess
{
    public static NativeProcess? TryGet(int processId)
    {
        var handle = NativeMethods.OpenProcess(0x1000, false, (uint)processId);

        if (handle == 0)
        {
            Debug.WriteLine($"Failed to open process #{processId}.");
            return null;
        }

        return new NativeProcess(handle);
    }
}
