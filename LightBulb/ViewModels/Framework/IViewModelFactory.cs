using LightBulb.ViewModels.Components;

namespace LightBulb.ViewModels.Framework
{
    // Used to instantiate new view models while making use of dependency injection
    public interface IViewModelFactory
    {
        GeneralSettingsViewModel CreateGeneralSettingsViewModel();

        LocationSettingsViewModel CreateLocationSettingsViewModel();

        AdvancedSettingsViewModel CreateAdvancedSettingsViewModel();
    }
}