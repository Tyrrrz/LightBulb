using System.Diagnostics;
using System.Text;
using LightBulb.WindowsApi.Native;

namespace LightBulb.WindowsApi;

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
    public static NativeProcess? TryOpen(int processId)
    {
        var handle = NativeMethods.OpenProcess(
            ProcessAccessFlags.QueryLimitedInformation,
            false,
            (uint)processId
        );
        if (handle == 0)
        {
            Debug.WriteLine($"Failed to open process (ID: {processId}).");
            return null;
        }

        return new NativeProcess(handle);
    }
}
