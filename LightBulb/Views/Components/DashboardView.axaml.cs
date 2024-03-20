using Avalonia.Input;
using Avalonia.Interactivity;
using LightBulb.ViewModels.Components;
using LightBulb.Views.Framework;

namespace LightBulb.Views.Components;

public partial class DashboardView : ViewModelAwareUserControl<DashboardViewModel>
{
    public DashboardView() => InitializeComponent();

    private void UserControl_OnLoaded(object? sender, RoutedEventArgs args) =>
        DataContext.InitializeCommand.Execute(null);

    private void ConfigurationOffsetStackPanel_OnPointerPressed(
        object? sender,
        PointerPressedEventArgs args
    ) => DataContext.ResetConfigurationOffsetCommand.Execute(null);
}
