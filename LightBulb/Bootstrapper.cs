using System;
using System.Threading;
using LightBulb.Services;
using LightBulb.ViewModels;
using LightBulb.ViewModels.Components;
using LightBulb.ViewModels.Dialogs;
using LightBulb.ViewModels.Framework;
using Stylet;
using StyletIoC;
using MessageBoxViewModel = LightBulb.ViewModels.Dialogs.MessageBoxViewModel;

#if !DEBUG
using System.Windows;
using System.Windows.Threading;
#endif

namespace LightBulb
{
    public class Bootstrapper : Bootstrapper<RootViewModel>
    {
        private readonly Mutex _identityMutex;
        private readonly bool _isOnlyRunningInstance;

        public Bootstrapper()
        {
            _identityMutex = new Mutex(true, "LightBulb_Identity", out _isOnlyRunningInstance);
        }

        public override void Start(string[] args)
        {
            // If there are other instances of this app running - exit
            if (!_isOnlyRunningInstance)
            {
                Environment.Exit(0);
                return;
            }

            base.Start(args);
        }

        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            // We don't call base method because that autobinds everything in transient scope.
            // We need to bind our dependencies as singletons due to extensive event routing.

            // Bind services
            builder.Bind<LocationService>().ToSelf().InSingletonScope();
            builder.Bind<SettingsService>().ToSelf().InSingletonScope();
            builder.Bind<GammaService>().ToSelf().InSingletonScope();
            builder.Bind<HotKeyService>().ToSelf().InSingletonScope();
            builder.Bind<RegistryService>().ToSelf().InSingletonScope();
            builder.Bind<WindowService>().ToSelf().InSingletonScope();
            builder.Bind<UpdateService>().ToSelf().InSingletonScope();

            // Bind view model layer services
            builder.Bind<DialogManager>().ToSelf().InSingletonScope();
            builder.Bind<IViewModelFactory>().ToAbstractFactory();

            // Bind view models
            builder.Bind<RootViewModel>().ToSelf().InSingletonScope();
            builder.Bind<MessageBoxViewModel>().ToSelf().InSingletonScope();
            builder.Bind<SettingsViewModel>().ToSelf().InSingletonScope();
            builder.Bind<ISettingsTabViewModel>().ToAllImplementations().InSingletonScope();
        }

        protected override void Launch()
        {
            // Finalize pending updates (and restart) before launching the app
            var updateService = (UpdateService) GetInstance(typeof(UpdateService));
            updateService.FinalizePendingUpdates();

            base.Launch();
        }

#if !DEBUG
        protected override void OnUnhandledException(DispatcherUnhandledExceptionEventArgs e)
        {
            base.OnUnhandledException(e);

            MessageBox.Show(e.Exception.ToString(), "Error occured", MessageBoxButton.OK, MessageBoxImage.Error);
        }
#endif

        public override void Dispose()
        {
            _identityMutex.Dispose();

            base.Dispose();
        }
    }
}