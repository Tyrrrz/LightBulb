using System;
using Stylet;

namespace LightBulb.ViewModels.Components
{
    public abstract class SettingsTabViewModelBase : PropertyChangedBase, ISettingsTabViewModel
    {
        public string DisplayName { get; protected set; }

        // This is purely for binding purposes
        public Type Type => GetType();

        public bool IsActive { get; set; }

        public int Order { get; protected set; }

        protected SettingsTabViewModelBase()
        {
            DisplayName = Type.ToString();
        }
    }
}