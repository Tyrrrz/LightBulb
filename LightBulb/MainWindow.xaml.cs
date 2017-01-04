using System.Windows;
using LightBulb.Services;
using NegativeLayer.Extensions;

namespace LightBulb
{
    public partial class MainWindow
    {
        private readonly WinApiService _winApiService = new WinApiService();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            _winApiService.RestoreOriginal();
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _winApiService.SetTemperature((ushort)e.NewValue.RoundToInt());
        }
    }
}