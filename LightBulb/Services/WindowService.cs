using System;
using LightBulb.WindowsApi;

namespace LightBulb.Services
{
    public class WindowService : IDisposable
    {
        private readonly WindowManager _windowManager = new WindowManager();

        public bool IsForegroundWindowFullScreen()
        {
            var foregroundWindow = _windowManager.GetForegroundWindow();
            return _windowManager.IsWindowFullScreen(foregroundWindow);
        }

        public void Dispose() => _windowManager.Dispose();
    }
}