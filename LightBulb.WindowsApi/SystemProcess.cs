﻿using System;
using System.Text;
using LightBulb.WindowsApi.Internal;

namespace LightBulb.WindowsApi
{
    public class SystemProcess : IDisposable
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
}