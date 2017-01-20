using System.Threading;
using System.Windows;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Win32;

namespace LightBulb
{
    public partial class App
    {
        private static Mutex _identityMutex;

        static App()
        {
            DispatcherHelper.Initialize();

            SystemEvents.SessionEnding += (sender, args) => Settings.Default.TrySave();
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            // Make sure only one process is running at a time
            // HACK: this waits until the app is fully started before closing it
            // ... would be better if we could prevent it starting entirely
            const string mutexName = "LightBulb_Identity";
            if (Mutex.TryOpenExisting(mutexName, out _identityMutex))
            {
                Shutdown();
                return;
            }
            _identityMutex = new Mutex(true, mutexName);

            Settings.Default.TryLoad();
        }

        private void App_OnExit(object sender, ExitEventArgs exitEventArgs)
        {
            Settings.Default.TrySave();
            Locator.Cleanup();
        }
    }
}