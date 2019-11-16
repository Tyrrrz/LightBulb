using LightBulb.WindowsApi;

namespace LightBulb.Services
{
    public class WindowService
    {
        public bool IsForegroundWindowFullScreen()
        {
            using var window = SystemWindow.GetForegroundWindow();
            return window.IsFullScreen();
        }

        public string GetForegroundWindowExecutableFilePath()
        {
            using var window = SystemWindow.GetForegroundWindow();
            using var process = window.GetProcess();

            return process.GetExecutableFilePath();
        }
    }
}