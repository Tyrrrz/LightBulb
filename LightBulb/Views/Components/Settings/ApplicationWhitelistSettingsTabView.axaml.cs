using Avalonia.Interactivity;
using LightBulb.Framework;
using LightBulb.ViewModels.Components.Settings;

namespace LightBulb.Views.Components.Settings;

public partial class ApplicationWhitelistSettingsTabView
    : UserControl<ApplicationWhitelistSettingsTabViewModel>
{
    public ApplicationWhitelistSettingsTabView() => InitializeComponent();

    private void UserControl_OnLoaded(object? sender, RoutedEventArgs args) =>
        DataContext.PullAvailableApplicationsCommand.Execute(null);
}
