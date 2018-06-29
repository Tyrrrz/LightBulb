using System;
using GalaSoft.MvvmLight.CommandWpf;
using LightBulb.Models;
using LightBulb.Services;

namespace LightBulb.ViewModels
{
    public interface IMainViewModel
    {
        /// <summary>
        /// Settings interface
        /// </summary>
        ISettingsService SettingsService { get; }

        /// <summary>
        /// Program version
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// Whether a new version of this program is available
        /// </summary>
        bool IsUpdateAvailable { get; }

        /// <summary>
        /// Enables or disables the program
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Whether gamma control is blocked by something 
        /// </summary>
        bool IsBlocked { get; }

        /// <summary>
        /// Current status text
        /// </summary>
        string StatusText { get; }

        /// <summary>
        /// Current state in the day cycle
        /// </summary>
        CycleState CycleState { get; }

        /// <summary>
        /// Current position in the day cycle
        /// </summary>
        double CyclePosition { get; }

        RelayCommand ExitApplicationCommand { get; }
        RelayCommand AboutCommand { get; }
        RelayCommand ToggleEnabledCommand { get; }
        RelayCommand<double> DisableTemporarilyCommand { get; }
        RelayCommand DownloadNewVersionCommand { get; }
    }
}