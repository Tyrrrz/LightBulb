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

    private void RootView_OnLoaded(object sender, RoutedEventArgs e)
    {
        // Hide to tray as soon as the window is loaded, if necessary
        if (App.IsHiddenOnLaunch)
            HideToTray();
    }

    private void RootView_OnClosing(object sender, CancelEventArgs e)
    {
        e.Cancel = true;
        HideToTray();
    }

    private void TaskbarIcon_OnTrayLeftMouseUp(object sender, RoutedEventArgs e)
    {
        RestoreFromTray();
    }

    private void ShowWindowMenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        RestoreFromTray();
    }

    private void Header_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            DragMove();
    }

    private void CloseButton_OnClick(object sender, RoutedEventArgs e)
    {
        HideToTray();
    }
}