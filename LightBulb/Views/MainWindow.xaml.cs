using System;
using System.Windows;
using System.Windows.Input;

namespace LightBulb.Views
{
    public partial class MainWindow
    {
        private DateTimeOffset _lastWindowHideTime = DateTimeOffset.MinValue;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            // Hide window initially
            Hide();
            _lastWindowHideTime = DateTimeOffset.Now;

            // Position window above the taskbar and at the edge of the screen
            Left = SystemParameters.WorkArea.Width - Width - 5;
            Top = SystemParameters.WorkArea.Height - Height;

            // Update nav buttons
            UpdateNavigationButtons();
        }

        private void MainWindow_OnDeactivated(object sender, EventArgs e)
        {
            // Hide window
            Hide();
            _lastWindowHideTime = DateTimeOffset.Now;
        }

        private void TaskbarIcon_OnTrayLeftMouseUp(object sender, RoutedEventArgs e)
        {
            // Do not show the window if it was hidden recently (avoid accidental clicks)
            if ((DateTimeOffset.Now - _lastWindowHideTime).TotalSeconds <= 0.1)
                return;

            // Show and activate window
            Show();
            Activate();
            Focus();
        }

        private void ConfigureMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            // Show and activate window
            Show();
            Activate();
            Focus();
        }

        private void Header_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void UpdateNavigationButtons()
        {
            if (ContentTransitioner == null) return;

            if (ContentTransitioner.SelectedIndex == 0)
                NavigateGeneralSettingsButton.SetResourceReference(BorderBrushProperty, "SecondaryAccentBrush");
            else
                NavigateGeneralSettingsButton.ClearValue(BorderBrushProperty);

            if (ContentTransitioner.SelectedIndex == 1)
                NavigateGeolocationSettingsButton.SetResourceReference(BorderBrushProperty, "SecondaryAccentBrush");
            else
                NavigateGeolocationSettingsButton.ClearValue(BorderBrushProperty);

            if (ContentTransitioner.SelectedIndex == 2)
                NavigateAdvancedSettingsButton.SetResourceReference(BorderBrushProperty, "SecondaryAccentBrush");
            else
                NavigateAdvancedSettingsButton.ClearValue(BorderBrushProperty);
        }

        private void NavigateGeneralSettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            ContentTransitioner.SelectedIndex = 0;
            UpdateNavigationButtons();
        }

        private void NavigateGeoSettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            ContentTransitioner.SelectedIndex = 1;
            UpdateNavigationButtons();
        }

        private void NavigateAdvancedSettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            ContentTransitioner.SelectedIndex = 2;
            UpdateNavigationButtons();
        }
    }
}