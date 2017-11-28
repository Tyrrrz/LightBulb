using System.Threading;
using System.Windows;
using GalaSoft.MvvmLight.Threading;
using LightBulb.Views;

namespace LightBulb
{
    public partial class App
    {
        private const string MutexName = "LightBulb_Identity";
        private static Mutex _identityMutex;

        static App()
        {
            DispatcherHelper.Initialize();
        }

        private bool _isInit;

        private void App_OnStartup(object sender, StartupEventArgs args)
        {
            // If already running - shutdown
            if (Mutex.TryOpenExisting(MutexName, out _identityMutex))
            {
                Shutdown();
            }
            // If not - proceed
            else
            {
                _identityMutex = new Mutex(true, MutexName);

                // Init locator
                Locator.Init();

                // Launch main window
                (MainWindow = new MainWindow()).Show();

                _isInit = true;
            }
        }

        private void App_OnExit(object sender, ExitEventArgs args)
        {
            if (_isInit)
                Locator.Cleanup();
        }
    }
}