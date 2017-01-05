using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using LightBulb.ViewModels;
using LiveCharts;
using NegativeLayer.Extensions;

namespace LightBulb.Views
{
    public partial class GeneralSettingsPage
    {
        public MainViewModel ViewModel => (MainViewModel) DataContext;

        private bool _maxTempSliderMouseDown;
        private bool _minTempSliderMouseDown;

        public GeneralSettingsPage()
        {
            InitializeComponent();
        }

        private void GeneralSettingsPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            ViewModel.PropertyChanged += (o, args) =>
            {
                if (args.PropertyName == nameof(ViewModel.IsPreviewModeEnabled) && !ViewModel.IsPreviewModeEnabled)
                    UpdatePlot();
            };
            ViewModel.Settings.PropertyChanged += (o, args) =>
            {
                if (!ViewModel.IsPreviewModeEnabled)
                    UpdatePlot();
            };

            UpdatePlot();
        }

        private void MaxTempSlider_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _maxTempSliderMouseDown = true;
            ViewModel.PreviewTemperature = (ushort) MaxTempSlider.Value;
            ViewModel.IsPreviewModeEnabled = true;
        }

        private void MaxTempSlider_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _maxTempSliderMouseDown = false;
            ViewModel.IsPreviewModeEnabled = false;
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
            ViewModel.IsPreviewModeEnabled = true;
        }

        private void MinTempSlider_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _minTempSliderMouseDown = false;
            ViewModel.IsPreviewModeEnabled = false;
        }

        private void MinTempSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_minTempSliderMouseDown)
                ViewModel.PreviewTemperature = (ushort) e.NewValue;
        }

        private void UpdatePlot()
        {
            // Brushes
            var gradient = new LinearGradientBrush(
                Color.FromRgb(244, 203, 66),
                Color.FromRgb(255, 255, 255),
                new Point(0, 1),
                new Point(0, 0)
            );
            PreviewPlot.Background = gradient;

            // Points
            var step = TimeSpan.FromHours(0.25);
            int count = (24/step.TotalHours).RoundToInt();
            PreviewPlotSeries.Values = new ChartValues<int>();
            for (int i = 0; i < count; i++)
            {
                var dt = DateTime.Today.AddHours(i*step.TotalHours);
                int temp = ViewModel.GetTemperature(dt);
                PreviewPlotSeries.Values.Add(temp);
            }
        }
    }
}