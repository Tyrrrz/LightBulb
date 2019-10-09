using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using LightBulb.Internal;
using LightBulb.Services;
using LightBulb.ViewModels;
using LightBulb.ViewModels.Components;
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

            // Bind all settings tabs
            builder.Bind<ISettingsTabViewModel>().ToAllImplementations();

            // Bind view model factory
            builder.Bind<IViewModelFactory>().ToAbstractFactory();
        }

        protected override void Launch()
        {
            // Finalize pending updates (and restart) before launching the app
            var updateService = (UpdateService) GetInstance(typeof(UpdateService));
            updateService.FinalizePendingUpdates();

            base.Launch();
        }

        protected override void OnUnhandledException(DispatcherUnhandledExceptionEventArgs e)
        {
            base.OnUnhandledException(e);

            if (!EnvironmentEx.IsDebug())
                MessageBox.Show(e.Exception.ToString(), "Error occured", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}