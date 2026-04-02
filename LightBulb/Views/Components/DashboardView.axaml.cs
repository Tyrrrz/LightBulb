using Avalonia.Input;
using LightBulb.Framework;
using LightBulb.ViewModels.Components;

namespace LightBulb.Views.Components;

public partial class DashboardView : UserControl<DashboardViewModel>
{
    public DashboardView() => InitializeComponent();

    private void ConfigurationOffsetStackPanel_OnPointerReleased(
        object? sender,
        PointerReleasedEventArgs args
    ) => DataContext.ResetConfigurationOffsetCommand.Execute(null);
}
