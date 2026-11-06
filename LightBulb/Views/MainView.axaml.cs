using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using LightBulb.Framework;
using LightBulb.ViewModels;

namespace LightBulb.Views;

public partial class MainView : Window<MainViewModel>
{
    public MainView() => InitializeComponent();

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsVisibleProperty && DataContext is not null)
            DataContext.IsOpen = IsVisible;
    }

    private void Window_OnLoaded(object? sender, RoutedEventArgs args)
    {
        // If the app is set to start hidden, hide the window, unless a dialog is open
        if (StartOptions.Current.IsInitiallyHidden && !DialogHost.IsOpen)
            Hide();
    }

    private void Window_OnClosing(object? sender, WindowClosingEventArgs args)
    {
        // When the user tries to close the window, hide it instead
        if (args.CloseReason == WindowCloseReason.WindowClosing)
        {
            args.Cancel = true;
            Hide();
        }
    }

    private void HeaderBorder_OnPointerPressed(object? sender, PointerPressedEventArgs args) =>
        BeginMoveDrag(args);

    private void HideButton_OnClick(object sender, RoutedEventArgs args) => Hide();
}
