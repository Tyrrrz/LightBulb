using Avalonia.Input;
using Avalonia.Interactivity;
using LightBulb.Framework;
using LightBulb.ViewModels;

namespace LightBulb.Views;

public partial class MainView : Window<MainViewModel>
{
    public MainView() => InitializeComponent();

    private void HeaderBorder_OnPointerPressed(object? sender, PointerPressedEventArgs args) =>
        BeginMoveDrag(args);

    private void HideButton_OnClick(object sender, RoutedEventArgs args) =>
        // The window is closed, but the backend and the tray icon will persist
        Close();
}
