using System;

namespace LightBulb.WindowsApi
{
    public class WindowManager : IDisposable
    {
        ~WindowManager()
        {
            Dispose();
        }

        public void Dispose()
        {
        }
    }
}