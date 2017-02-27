using LightBulb.Services.Interfaces;

namespace LightBulb.ViewModels.Interfaces
{
    public interface IAdvancedSettingsViewModel
    {
        /// <summary>
        /// Settings interface
        /// </summary>
        ISettingsService SettingsService { get; }
    }
}