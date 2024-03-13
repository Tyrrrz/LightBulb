using System;
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

    private void MainView_OnLoaded(object sender, RoutedEventArgs args)
    {
        // Hide to tray as soon as the window is loaded, if necessary
        if (App.IsHiddenOnLaunch)
            HideToTray();
    }

    private void MainView_OnClosing(object sender, CancelEventArgs args)
    {
        args.Cancel = true;
        HideToTray();
    }

    private void TrayIcon_OnClicked(object? sender, EventArgs args)
    {
        RestoreFromTray();
    }

    private void ShowWindowMenuItem_OnClick(object? sender, EventArgs args)
    {
        RestoreFromTray();
    }

    private void Header_OnPointerPressed(object? sender, PointerPressedEventArgs args)
    {
        BeginMoveDrag(args);
    }

    private void HideButton_OnClick(object sender, RoutedEventArgs args)
    {
        HideToTray();
    }
}
