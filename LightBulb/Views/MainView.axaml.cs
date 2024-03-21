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
        if (StartupOptions.Current.IsInitiallyHidden)
            Hide();
    }

    private void Window_OnClosing(object sender, WindowClosingEventArgs args)
    {
        args.Cancel = true;
        Hide();
    }

    private void DialogHost_OnLoaded(object? sender, RoutedEventArgs args) =>
        DataContext.InitializeCommand.Execute(null);

    private void Header_OnPointerPressed(object? sender, PointerPressedEventArgs args) =>
        BeginMoveDrag(args);

    private void HideButton_OnClick(object sender, RoutedEventArgs args) => Hide();
}
