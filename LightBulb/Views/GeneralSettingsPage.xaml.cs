using System.Windows;
using System.Windows.Input;
using LightBulb.ViewModels;

namespace LightBulb.Views
{
    public partial class GeneralSettingsPage
    {
        public GeneralSettingsViewModel ViewModel => (GeneralSettingsViewModel) DataContext;

        private bool _maxTempSliderMouseDown;
        private bool _minTempSliderMouseDown;

        public GeneralSettingsPage()
        {
            InitializeComponent();
        }

        private void MaxTempSlider_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _maxTempSliderMouseDown = true;
            ViewModel.PreviewTemperature = (ushort) MaxTempSlider.Value;
            ViewModel.IsInPreviewMode = true;
        }

        private void MaxTempSlider_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _maxTempSliderMouseDown = false;
            ViewModel.IsInPreviewMode = false;
        }

        private void MaxTempSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_maxTempSliderMouseDown)
                ViewModel.PreviewTemperature = (ushort) e.NewValue;
        }

        private void MinTempSlider_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _minTempSliderMouseDown = true;
            ViewModel.PreviewTemperature = (ushort) MinTempSlider.Value;
            ViewModel.IsInPreviewMode = true;
        }

        private void MinTempSlider_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _minTempSliderMouseDown = false;
            ViewModel.IsInPreviewMode = false;
        }

        private void MinTempSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_minTempSliderMouseDown)
                ViewModel.PreviewTemperature = (ushort) e.NewValue;
        }
    }
}