using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using LightBulb.ViewModels.Components;

namespace LightBulb.Views.Components;

public partial class DashboardView : UserControl
{
    public DashboardView() => InitializeComponent();

    private void Root_OnLoaded(object? sender, RoutedEventArgs args) =>
        (DataContext as DashboardViewModel)?.InitializeCommand.Execute(null);

    private void ConfigurationOffsetStackPanel_OnPointerPressed(
        object? sender,
        PointerPressedEventArgs args
    ) => (DataContext as DashboardViewModel)?.ResetConfigurationOffsetCommand.Execute(null);
}
