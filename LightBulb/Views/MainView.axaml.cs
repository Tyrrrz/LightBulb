using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using LightBulb.ViewModels;

namespace LightBulb.Views;

public partial class MainView : Window
{
    public MainView() => InitializeComponent();

    private void Root_OnLoaded(object sender, RoutedEventArgs args)
    {
        // Hide to tray as soon as the window is loaded, if necessary
        if (App.IsHiddenOnLaunch)
            Hide();
    }

    private void Root_OnClosing(object sender, WindowClosingEventArgs args)
    {
        args.Cancel = true;
        Hide();
    }

    private void DialogHost_OnLoaded(object? sender, RoutedEventArgs args) =>
        (DataContext as MainViewModel)?.InitializeCommand.Execute(null);

    private void Header_OnPointerPressed(object? sender, PointerPressedEventArgs args) =>
        BeginMoveDrag(args);

    private void HideButton_OnClick(object sender, RoutedEventArgs args) => Hide();
}
