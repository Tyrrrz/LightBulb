using LightBulb.ViewModels.Dialogs;

namespace LightBulb.ViewModels.Framework
{
    // Used to instantiate new view models while making use of dependency injection
    public interface IViewModelFactory
    {
        MessageBoxViewModel CreateMessageBoxViewModel();

        SettingsViewModel CreateSettingsViewModel();
    }
}