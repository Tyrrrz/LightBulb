using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace LightBulb.Views;

public partial class MainView : Window
{
    public MainView()
    {
        InitializeComponent();
    }

    private void MainView_OnLoaded(object sender, RoutedEventArgs args)
    {
        // Hide to tray as soon as the window is loaded, if necessary
        if (App.IsHiddenOnLaunch)
            Hide();
    }

    private void MainView_OnClosing(object sender, CancelEventArgs args)
    {
        args.Cancel = true;
        Hide();
    }

    private void Header_OnPointerPressed(object? sender, PointerPressedEventArgs args) => 
        BeginMoveDrag(args);

    private void HideButton_OnClick(object sender, RoutedEventArgs args) => Hide();
}
