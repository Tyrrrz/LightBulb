using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LightBulb.Services.Abstract
{
    public abstract class WinApiServiceBase
    {
        protected Win32Exception GetLastError()
        {
            int errCode = Marshal.GetLastWin32Error();
            if (errCode == 0) return null;
            return new Win32Exception(errCode);
        }

        protected void CheckLogWin32Error()
        {
            var ex = GetLastError();
            if (ex != null) Debug.WriteLine($"Win32 error: {ex.Message} ({ex.NativeErrorCode})", GetType().Name);
        }
    }
}
