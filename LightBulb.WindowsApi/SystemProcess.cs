using System;
using System.Text;
using LightBulb.WindowsApi.Internal;

namespace LightBulb.WindowsApi
{
    public class SystemProcess
    {
        public IntPtr Handle { get; }

        public SystemProcess(IntPtr handle)
        {
            Handle = handle;
        }

        public string GetExecutableFilePath()
        {
            var buffer = new StringBuilder(1024);
            var bufferSize = (uint) buffer.Capacity + 1;

            // This can return empty string
            NativeMethods.QueryFullProcessImageName(Handle, 0, buffer, ref bufferSize);

            return buffer.ToString();
        }
    }
}