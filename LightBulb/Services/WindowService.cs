using LightBulb.WindowsApi;

namespace LightBulb.Services
{
    public class WindowService
    {
        public bool IsForegroundWindowFullScreen() => SystemWindow.GetForegroundWindow().IsFullScreen();

        public string GetForegroundWindowExecutableFilePath() => SystemWindow.GetForegroundWindow().GetProcess().GetExecutableFilePath();
    }
}