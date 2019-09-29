using System;
using System.Windows;
using System.Windows.Input;

namespace LightBulb.Views
{
    public partial class RootView
    {
        private DateTimeOffset _lastWindowHideTime = DateTimeOffset.MinValue;

        public RootView()
        {
            InitializeComponent();
        }

        private void HideToTray()
        {
            // Hide window
            Hide();

            // Record the time
            _lastWindowHideTime = DateTimeOffset.Now;
        }

        private void RestoreFromTray()
        {
            // Do not restore the window if it was hidden recently (avoid accidental double clicks)
            if ((DateTimeOffset.Now - _lastWindowHideTime).TotalSeconds <= 0.1)
                return;

            // Show and activate window
            Show();
            Activate();
            Focus();
        }

        private void TaskbarIcon_OnTrayLeftMouseUp(object sender, RoutedEventArgs routedEventArgs)
        {
            RestoreFromTray();
        }

        private void ConfigureMenuItem_OnClick(object sender, RoutedEventArgs e)
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
}