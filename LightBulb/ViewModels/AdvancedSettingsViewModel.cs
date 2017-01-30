using GalaSoft.MvvmLight;

namespace LightBulb.ViewModels
{
    public class AdvancedSettingsViewModel : ViewModelBase
    {
        public Settings Settings => Settings.Default;
    }
}