using System.Diagnostics;
using System.Text;
using LightBulb.WindowsApi.Types;

namespace LightBulb.WindowsApi;

public partial class SystemProcess : NativeResource
{
    private SystemProcess(nint handle)
        : base(handle)
    {
    }

    public string? TryGetExecutableFilePath()
    {
        var buffer = new StringBuilder(1024);
        var bufferSize = (uint) buffer.Capacity + 1;

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

public partial class SystemProcess
{
    public static SystemProcess? TryOpen(int processId)
    {
        var handle = NativeMethods.OpenProcess(ProcessAccessFlags.QueryLimitedInformation, false, (uint) processId);
        if (handle == 0)
        {
            Debug.WriteLine($"Failed to open process (ID: {processId}).");
            return null;
        }

        return new SystemProcess(handle);
    }
}