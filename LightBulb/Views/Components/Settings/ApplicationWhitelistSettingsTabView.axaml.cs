using Avalonia.Controls;
using Avalonia.Interactivity;
using LightBulb.ViewModels.Components.Settings;

namespace LightBulb.Views.Components.Settings;

public partial class ApplicationWhitelistSettingsTabView : UserControl
{
    public ApplicationWhitelistSettingsTabView() => InitializeComponent();

    private void Root_OnLoaded(object? sender, RoutedEventArgs args) =>
        (DataContext as ApplicationWhitelistSettingsTabViewModel)
            ?.PullAvailableApplicationsCommand
            .Execute(null);
}
