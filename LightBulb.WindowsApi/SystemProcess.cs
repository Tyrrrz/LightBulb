using System;
using System.Collections.Generic;
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
            if (!NativeMethods.CloseHandle(Handle))
                Debug.WriteLine("Could not dispose process.");

            GC.SuppressFinalize(this);
        }
    }

    public partial class SystemProcess
    {
        public static SystemProcess? Open(int processId)
        {
            var handle = NativeMethods.OpenProcess(ProcessAccessFlags.QueryLimitedInformation, false, processId);
            return handle != IntPtr.Zero ? new SystemProcess(handle) : null;
        }

        // TODO: use native call
        public static IReadOnlyList<SystemProcess> GetAllWindowedProcesses()
        {
            var result = new List<SystemProcess>();

            foreach (var process in Process.GetProcesses())
            {
                using var _ = process;

                var systemProcess = Open(process.Id);
                if (systemProcess != null)
                    result.Add(systemProcess);
            }

            return result;
        }
    }
}