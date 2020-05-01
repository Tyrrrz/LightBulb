using System;
using System.Threading;
using LightBulb.Services;
using LightBulb.ViewModels;
using LightBulb.ViewModels.Components;
using LightBulb.ViewModels.Components.Settings;
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

        private T GetInstance<T>() => (T) base.GetInstance(typeof(T));

        public override void Start(string[] args)
        {
            // Ensure only one instance of the app is running at a time
            if (!_isOnlyRunningInstance)
            {
                Environment.Exit(0);
                return;
            }

            base.Start(args);
        }

        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            base.ConfigureIoC(builder);

            builder.Bind<LocationService>().ToSelf().InSingletonScope();
            builder.Bind<SettingsService>().ToSelf().InSingletonScope();
            builder.Bind<GammaService>().ToSelf().InSingletonScope();
            builder.Bind<HotKeyService>().ToSelf().InSingletonScope();
            builder.Bind<ExternalApplicationService>().ToSelf().InSingletonScope();
            builder.Bind<UpdateService>().ToSelf().InSingletonScope();

            builder.Bind<DialogManager>().ToSelf().InSingletonScope();
            builder.Bind<IViewModelFactory>().ToAbstractFactory();

            builder.Bind<RootViewModel>().ToSelf().InSingletonScope();
            builder.Bind<CoreViewModel>().ToSelf().InSingletonScope();
            builder.Bind<MessageBoxViewModel>().ToSelf().InSingletonScope();
            builder.Bind<SettingsViewModel>().ToSelf().InSingletonScope();
            builder.Bind<ISettingsTabViewModel>().ToAllImplementations().InSingletonScope();
        }

        protected override void Launch()
        {
            // Finalize pending updates (and restart) before launching the app
            GetInstance<UpdateService>().FinalizePendingUpdates();

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