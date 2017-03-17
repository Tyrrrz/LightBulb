using GalaSoft.MvvmLight.CommandWpf;
using LightBulb.Services.Interfaces;

namespace LightBulb.ViewModels.Interfaces
{
    public interface IGeneralSettingsViewModel
    {
        /// <summary>
        /// Settings interface
        /// </summary>
        ISettingsService SettingsService { get; }

        /// <summary>
        /// Enables or disables the preview mode
        /// </summary>
        bool IsPreviewModeEnabled { get; set; }

        /// <summary>
        /// Enables or disables cycle preview mode
        /// </summary>
        bool IsCyclePreviewRunning { get; set; }

        RelayCommand StartStopCyclePreviewCommand { get; }
    }
}