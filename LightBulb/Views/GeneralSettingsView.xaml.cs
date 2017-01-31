using System.Windows;
using System.Windows.Input;
using LightBulb.ViewModels;

namespace LightBulb.Views
{
    public partial class GeneralSettingsView
    {
        public GeneralSettingsViewModel ViewModel => (GeneralSettingsViewModel) DataContext;

        private bool _maxTempSliderMouseDown;
        private bool _minTempSliderMouseDown;

        public GeneralSettingsView()
        {
            InitializeComponent();
        }

        private void MaxTempSlider_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;

            _maxTempSliderMouseDown = true;
            ViewModel.RequestPreviewTemperatureCommand.Execute((ushort) MaxTempSlider.Value);
            ViewModel.IsInPreviewMode = true;
        }

        private void MaxTempSlider_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;

            _maxTempSliderMouseDown = false;
            ViewModel.IsInPreviewMode = false;
        }

        private void MaxTempSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_maxTempSliderMouseDown)
                ViewModel.RequestPreviewTemperatureCommand.Execute((ushort) e.NewValue);
        }

        private void MinTempSlider_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;

            _minTempSliderMouseDown = true;
            ViewModel.RequestPreviewTemperatureCommand.Execute((ushort) MinTempSlider.Value);
            ViewModel.IsInPreviewMode = true;
        }

        private void MinTempSlider_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;

            _minTempSliderMouseDown = false;
            ViewModel.IsInPreviewMode = false;
        }

        private void MinTempSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_minTempSliderMouseDown)
                ViewModel.RequestPreviewTemperatureCommand.Execute((ushort) e.NewValue);
        }
    }
}