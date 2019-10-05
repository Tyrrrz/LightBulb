using System;

namespace LightBulb.ViewModels.Components
{
    public interface ISettingsTabViewModel
    {
        int Order { get; }

        string DisplayName { get; }

        Type Type { get; }

        bool IsActive { get; set; }
    }
}