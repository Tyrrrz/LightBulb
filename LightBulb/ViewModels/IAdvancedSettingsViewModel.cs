using LightBulb.Services;

namespace LightBulb.ViewModels
{
    public interface IAdvancedSettingsViewModel
    {
        /// <summary>
        /// Settings interface
        /// </summary>
        ISettingsService SettingsService { get; }
    }
}