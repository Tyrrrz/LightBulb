using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        ~SystemProcess()
        {
            Dispose();
        }

        public string? GetExecutableFilePath()
        {
            var buffer = new StringBuilder(1024);
            var bufferSize = (uint) buffer.Capacity + 1;

            return NativeMethods.QueryFullProcessImageName(Handle, 0, buffer, ref bufferSize) ? buffer.ToString() : null;
        }

        public void Dispose()
        {
            // Potentially unhandled error
            NativeMethods.CloseHandle(Handle);

            GC.SuppressFinalize(this);
        }
    }

    public partial class SystemProcess
    {
        public static SystemProcess Open(int processId)
        {
            // Potentially unhandled error
            var handle = NativeMethods.OpenProcess(ProcessAccessFlags.QueryLimitedInformation, false, processId);
            return new SystemProcess(handle);
        }

        // TODO: use native call
        public static IReadOnlyList<SystemProcess> GetAllWindowedProcesses() =>
            Process.GetProcesses()
                .Where(p => p.MainWindowHandle != IntPtr.Zero)
                .Select(p =>
                {
                    using (p)
                        return Open(p.Id);
                })
                .ToArray();
    }
}