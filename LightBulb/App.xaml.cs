using System.Windows;
using GalaSoft.MvvmLight.Threading;

namespace LightBulb
{
    public partial class App
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }

        private void App_OnExit(object sender, ExitEventArgs exitEventArgs)
        {
            Locator.Cleanup();
        }
    }
}