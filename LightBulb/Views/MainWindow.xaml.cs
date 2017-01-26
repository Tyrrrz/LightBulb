using System;
using System.Windows;
using System.Windows.Input;

namespace LightBulb.Views
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Hide();

            Left = SystemParameters.WorkArea.Width - Width - 5;
            Top = SystemParameters.WorkArea.Height - Height;

            NavigateGeneralSettingsButton.IsEnabled = false;
        }

        private void MainWindow_OnDeactivated(object sender, EventArgs e)
        {
            Hide();
        }

        private void HeaderBorder_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void NavigateGeneralSettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new GeneralSettingsPage());

            NavigateGeneralSettingsButton.IsEnabled = false;
            NavigateGeolocationSettingsButton.IsEnabled = true;
            NavigateAdvancedSettingsButton.IsEnabled = true;
        }

        private void NavigateGeolocationSettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new GeolocationSettingsPage());

            NavigateGeneralSettingsButton.IsEnabled = true;
            NavigateGeolocationSettingsButton.IsEnabled = false;
            NavigateAdvancedSettingsButton.IsEnabled = true;
        }

        private void NavigateAdvancedSettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new AdvancedSettingsPage());

            NavigateGeneralSettingsButton.IsEnabled = true;
            NavigateGeolocationSettingsButton.IsEnabled = true;
            NavigateAdvancedSettingsButton.IsEnabled = false;
        }
    }
}