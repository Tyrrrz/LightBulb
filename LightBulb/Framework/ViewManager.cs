using Avalonia.Controls;
using Avalonia.Controls.Templates;
using LightBulb.ViewModels;
using LightBulb.ViewModels.Components;
using LightBulb.ViewModels.Components.Settings;
using LightBulb.ViewModels.Dialogs;
using LightBulb.Views;
using LightBulb.Views.Components;
using LightBulb.Views.Components.Settings;
using LightBulb.Views.Dialogs;

namespace LightBulb.Framework;

public partial class ViewManager
{
    private Control? TryCreateView(ViewModelBase viewModel) =>
        viewModel switch
        {
            MainViewModel => new MainView(),
            DashboardViewModel => new DashboardView(),
            MessageBoxViewModel => new MessageBoxView(),
            SettingsViewModel => new SettingsView(),
            AdvancedSettingsTabViewModel => new AdvancedSettingsTabView(),
            ApplicationWhitelistSettingsTabViewModel => new ApplicationWhitelistSettingsTabView(),
            GeneralSettingsTabViewModel => new GeneralSettingsTabView(),
            HotKeySettingsTabViewModel => new HotKeySettingsTabView(),
            LocationSettingsTabViewModel => new LocationSettingsTabView(),
            _ => null
        };

    public Control? TryBindView(ViewModelBase viewModel)
    {
        var view = TryCreateView(viewModel);
        if (view is null)
            return null;

        view.DataContext ??= viewModel;

        return view;
    }
}

public partial class ViewManager : IDataTemplate
{
    bool IDataTemplate.Match(object? data) => data is ViewModelBase;

    Control? ITemplate<object?, Control?>.Build(object? data) =>
        data is ViewModelBase viewModel ? TryBindView(viewModel) : null;
}
