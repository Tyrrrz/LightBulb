using System;
using Stylet;

namespace LightBulb.ViewModels.Components
{
    public abstract class SettingsTabViewModel : PropertyChangedBase
    {
        public string DisplayName { get; protected set; }

        // This is purely for binding purposes
        public Type Type => GetType();

        public bool IsActive { get; set; }

        protected SettingsTabViewModel()
        {
            DisplayName = Type.ToString();
        }
    }
}