using System;

namespace LightBulb.ViewModels.Components
{
    public interface ISettingsTabViewModel
    {
        string DisplayName { get; }

        Type Type { get; }

        bool IsActive { get; set; }

        int Order { get; }
    }
}