using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace LightBulb.Views;

public partial class RootView
{
    public RootView()
    {
        InitializeComponent();
    }

    private void HideToTray()
    {
        Hide();
    }

    private void RestoreFromTray()
    {
        Show();
        Activate();
        Focus();
    }

    private void RootView_OnLoaded(object sender, RoutedEventArgs args)
    {
        // Hide to tray as soon as the window is loaded, if necessary
        if (App.IsHiddenOnLaunch)
            HideToTray();
    }

    private void RootView_OnClosing(object sender, CancelEventArgs args)
    {
        args.Cancel = true;
        HideToTray();
    }

    private void TaskbarIcon_OnTrayLeftMouseUp(object sender, RoutedEventArgs args)
    {
        RestoreFromTray();
    }

    private void ShowWindowMenuItem_OnClick(object sender, RoutedEventArgs args)
    {
        RestoreFromTray();
    }

    private void Header_OnMouseDown(object sender, MouseButtonEventArgs args)
    {
        if (args.ChangedButton == MouseButton.Left)
            DragMove();
    }

    private void CloseButton_OnClick(object sender, RoutedEventArgs args)
    {
        HideToTray();
    }
}