using LightBulb.WindowsApi;

namespace LightBulb.Services
{
    public class WindowService
    {
        public bool IsForegroundWindowFullScreen() => NativeWindow.GetForegroundWindow().IsFullScreen();

        public string GetForegroundWindowExecutableFilePath() => NativeWindow.GetForegroundWindow().GetProcess().GetExecutableFilePath();
    }
}