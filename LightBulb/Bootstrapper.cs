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
using System;
using System.Windows;
using System.Windows.Threading;
#endif

namespace LightBulb;

public class Bootstrapper : Bootstrapper<RootViewModel>
{
    private readonly Mutex _identityMutex;
    private readonly bool _isOnlyRunningInstance;

    public Bootstrapper()
    {
        _identityMutex = new Mutex(true, "LightBulb_Identity", out _isOnlyRunningInstance);
    }

    private T GetInstance<T>() => (T)base.GetInstance(typeof(T));

    public override void Start(string[] args)
    {
        // Ensure only one instance of the app is running at a time
        if (!_isOnlyRunningInstance)
        {
#if !DEBUG
            Environment.Exit(0);
            return;
#endif
        }

        base.Start(args);
    }

    protected override void ConfigureIoC(IStyletIoCBuilder builder)
    {
        base.ConfigureIoC(builder);

        builder.Bind<SettingsService>().ToSelf().InSingletonScope();
        builder.Bind<GammaService>().ToSelf().InSingletonScope();
        builder.Bind<HotKeyService>().ToSelf().InSingletonScope();
        builder.Bind<ExternalApplicationService>().ToSelf().InSingletonScope();
        builder.Bind<UpdateService>().ToSelf().InSingletonScope();

        builder.Bind<DialogManager>().ToSelf().InSingletonScope();
        builder.Bind<IViewModelFactory>().ToAbstractFactory();

        builder.Bind<RootViewModel>().ToSelf().InSingletonScope();
        builder.Bind<DashboardViewModel>().ToSelf().InSingletonScope();
        builder.Bind<MessageBoxViewModel>().ToSelf().InSingletonScope();
        builder.Bind<SettingsViewModel>().ToSelf().InSingletonScope();
        builder.Bind<ISettingsTabViewModel>().ToAllImplementations().InSingletonScope();
    }

    protected override void Launch()
    {
        // Finalize pending updates (and restart) before launching the app
        GetInstance<UpdateService>()
            .FinalizePendingUpdates();

        // Load settings (this has to come before any view is loaded because bindings are not updated)
        GetInstance<SettingsService>()
            .Load();

        // Stylet/WPF is slow, so we preload all dialogs, including descendants, for smoother UX
        _ = GetInstance<DialogManager>().GetViewForDialogScreen(GetInstance<SettingsViewModel>());
        _ = GetInstance<DialogManager>().GetViewForDialogScreen(GetInstance<MessageBoxViewModel>());

        base.Launch();
    }

#if !DEBUG
    protected override void OnUnhandledException(DispatcherUnhandledExceptionEventArgs args)
    {
        base.OnUnhandledException(args);

        MessageBox.Show(
            args.Exception.ToString(),
            "Error occured",
            MessageBoxButton.OK,
            MessageBoxImage.Error
        );
    }
#endif

    public override void Dispose()
    {
        _identityMutex.Dispose();
        base.Dispose();
    }
}
