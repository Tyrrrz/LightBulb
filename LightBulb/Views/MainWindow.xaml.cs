using System;
using System.Windows;
using System.Windows.Input;

namespace LightBulb.Views
{
    public partial class MainWindow
    {
        private readonly GeneralSettingsView _generalSettingsView;
        private readonly GeoSettingsView _geoSettingsView;
        private readonly AdvancedSettingsView _advancedSettingsView;

        public MainWindow()
        {
            InitializeComponent();

            _generalSettingsView = new GeneralSettingsView();
            _geoSettingsView = new GeoSettingsView();
            _advancedSettingsView = new AdvancedSettingsView();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Hide();

            Left = SystemParameters.WorkArea.Width - Width - 5;
            Top = SystemParameters.WorkArea.Height - Height;

            ViewPresenter.Content = _generalSettingsView;
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
            if (ReferenceEquals(ViewPresenter.Content, _generalSettingsView))
                NavigateGeneralSettingsButton.SetResourceReference(BorderBrushProperty, "SecondaryAccentBrush");
            else
                NavigateGeneralSettingsButton.ClearValue(BorderBrushProperty);

            if (ReferenceEquals(ViewPresenter.Content, _geoSettingsView))
                NavigateGeolocationSettingsButton.SetResourceReference(BorderBrushProperty, "SecondaryAccentBrush");
            else
                NavigateGeolocationSettingsButton.ClearValue(BorderBrushProperty);

            if (ReferenceEquals(ViewPresenter.Content, _advancedSettingsView))
                NavigateAdvancedSettingsButton.SetResourceReference(BorderBrushProperty, "SecondaryAccentBrush");
            else
                NavigateAdvancedSettingsButton.ClearValue(BorderBrushProperty);
        }

        private void NavigateGeneralSettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            ViewPresenter.Content = _generalSettingsView;
            UpdateNavigationButtons();
        }

        private void NavigateGeoSettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            ViewPresenter.Content = _geoSettingsView;
            UpdateNavigationButtons();
        }

        private void NavigateAdvancedSettingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            ViewPresenter.Content = _advancedSettingsView;
            UpdateNavigationButtons();
        }
    }
}