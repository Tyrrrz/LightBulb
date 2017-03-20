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

            UpdateNavigationButtons();
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