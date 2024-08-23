using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using LightBulb.Framework;
using LightBulb.ViewModels;

namespace LightBulb.Views;

public partial class MainView : Window<MainViewModel>
{
    public MainView() => InitializeComponent();

    private void Window_OnLoaded(object sender, RoutedEventArgs args)
    {
        // If the app is set to start hidden, hide the window, unless a dialog is open
        if (StartOptions.Current.IsInitiallyHidden && !DialogHost.IsOpen)
            Hide();
    }

    private void Window_OnClosing(object sender, WindowClosingEventArgs args)
    {
        // When the user tries to close the window, hide it instead
        if (args.CloseReason == WindowCloseReason.WindowClosing)
        {
            args.Cancel = true;
            Hide();
        }
    }

    private void DialogHost_OnLoaded(object? sender, RoutedEventArgs args) =>
        DataContext.InitializeCommand.Execute(null);

    private void HeaderBorder_OnPointerPressed(object? sender, PointerPressedEventArgs args) =>
        BeginMoveDrag(args);

    private void HideButton_OnClick(object sender, RoutedEventArgs args) => Hide();
}
