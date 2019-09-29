using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using LightBulb.Services;
using LightBulb.ViewModels;
using LightBulb.ViewModels.Framework;
using Stylet;
using StyletIoC;

namespace LightBulb
{
    public class Bootstrapper : Bootstrapper<RootViewModel>
    {
        // ReSharper disable once NotAccessedField.Local (need to keep reference)
        private static Mutex _identityMutex;

        public override void Start(string[] args)
        {
            // Ensure this is the only running instance, otherwise - exit
            _identityMutex = new Mutex(true, "LightBulb_Identity", out var isOnlyRunningInstance);
            if (!isOnlyRunningInstance)
            {
                Environment.Exit(0);
                return;
            }

            base.Start(args);
        }

        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            base.ConfigureIoC(builder);

            // Bind settings as singleton
            builder.Bind<SettingsService>().ToSelf().InSingletonScope();

            // Bind view model factory
            builder.Bind<IViewModelFactory>().ToAbstractFactory();
        }

#if !DEBUG
        protected override void OnUnhandledException(DispatcherUnhandledExceptionEventArgs e)
        {
            base.OnUnhandledException(e);

            MessageBox.Show(e.Exception.ToString(), "Error occured", MessageBoxButton.OK, MessageBoxImage.Error);
        }
#endif
    }
}