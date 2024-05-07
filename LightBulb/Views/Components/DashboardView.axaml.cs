using Avalonia.Input;
using Avalonia.Interactivity;
using LightBulb.Framework;
using LightBulb.ViewModels.Components;

namespace LightBulb.Views.Components;

public partial class DashboardView : UserControl<DashboardViewModel>
{
    public DashboardView() => InitializeComponent();

    private void UserControl_OnLoaded(object? sender, RoutedEventArgs args) =>
        DataContext.InitializeCommand.Execute(null);

    private void ConfigurationOffsetStackPanel_OnPointerReleased(
        object? sender,
        PointerReleasedEventArgs args
    ) => DataContext.ResetConfigurationOffsetCommand.Execute(null);
}
