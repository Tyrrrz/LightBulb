using System;
using System.Diagnostics;
using System.Text;
using LightBulb.WindowsApi.Internal;

namespace LightBulb.WindowsApi
{
    public partial class SystemProcess : IDisposable
    {
        public IntPtr Handle { get; }

        public SystemProcess(IntPtr handle)
        {
            Handle = handle;
        }

        ~SystemProcess() => Dispose();

        public string? GetExecutableFilePath()
        {
            var buffer = new StringBuilder(1024);
            var bufferSize = (uint) buffer.Capacity + 1;

            return NativeMethods.QueryFullProcessImageName(Handle, 0, buffer, ref bufferSize) ? buffer.ToString() : null;
        }

        public void Dispose()
        {
            if (!NativeMethods.CloseHandle(Handle))
                Debug.WriteLine("Could not dispose process.");

            GC.SuppressFinalize(this);
        }
    }

    public partial class SystemProcess
    {
        public static SystemProcess? TryOpen(uint processId)
        {
            var handle = NativeMethods.OpenProcess(ProcessAccessFlags.QueryLimitedInformation, false, processId);
            return handle != IntPtr.Zero ? new SystemProcess(handle) : null;
        }
    }
}