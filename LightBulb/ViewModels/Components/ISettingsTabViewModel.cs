namespace LightBulb.ViewModels.Components
{
    public interface ISettingsTabViewModel
    {
        int Order { get; }

        string DisplayName { get; }

        bool IsActive { get; set; }
    }
}