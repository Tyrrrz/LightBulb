using System.Threading;
using System.Windows;
using GalaSoft.MvvmLight.Threading;
using LightBulb.Views;
using Microsoft.Win32;
using Tyrrrz.WpfExtensions;

namespace LightBulb
{
    public partial class App
    {
        private static Mutex _identityMutex;

        static App()
        {
            DispatcherHelper.Initialize();

            // When windows session is ending - close application to execute cleanups and dumps
            SystemEvents.SessionEnding += (o, args) => Current.ShutdownSafe();
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            // Make sure only one process is running at a time
            const string mutexName = "LightBulb_Identity";
            if (Mutex.TryOpenExisting(mutexName, out _identityMutex))
            {
                Shutdown();
                return;
            }
            _identityMutex = new Mutex(true, mutexName);

            // Init locator
            Locator.Init();

            // Launch main window
            (MainWindow = new MainWindow()).Show();
        }

        private void App_OnExit(object sender, ExitEventArgs exitEventArgs)
        {
            Locator.Cleanup();
        }
    }
}