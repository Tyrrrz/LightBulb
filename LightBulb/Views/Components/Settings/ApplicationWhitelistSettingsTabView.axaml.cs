using Avalonia.Interactivity;
using LightBulb.ViewModels.Components.Settings;
using LightBulb.Views.Framework;

namespace LightBulb.Views.Components.Settings;

public partial class ApplicationWhitelistSettingsTabView
    : ViewModelAwareUserControl<ApplicationWhitelistSettingsTabViewModel>
{
    public ApplicationWhitelistSettingsTabView() => InitializeComponent();

    private void UserControl_OnLoaded(object? sender, RoutedEventArgs args) =>
        DataContext.PullAvailableApplicationsCommand.Execute(null);
}
