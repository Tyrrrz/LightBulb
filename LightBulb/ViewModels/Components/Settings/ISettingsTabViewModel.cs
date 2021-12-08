namespace LightBulb.ViewModels.Components.Settings;

public interface ISettingsTabViewModel
{
    int Order { get; }

    string DisplayName { get; }

    bool IsActive { get; set; }
}