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

        public string GetExecutableFilePath()
        {
            var buffer = new StringBuilder(1024);
            var bufferSize = (uint) buffer.Capacity + 1;

            // This can return empty string
            NativeMethods.QueryFullProcessImageName(Handle, 0, buffer, ref bufferSize);

            return buffer.ToString();
        }

        public void Dispose()
        {
            NativeMethods.CloseHandle(Handle);
            GC.SuppressFinalize(this);
        }
    }

    public partial class SystemProcess
    {
        public static SystemProcess Open(int processId)
        {
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