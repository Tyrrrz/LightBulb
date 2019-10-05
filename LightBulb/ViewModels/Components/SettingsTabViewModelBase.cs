﻿using System;
using Stylet;

namespace LightBulb.ViewModels.Components
{
    public abstract class SettingsTabViewModelBase : PropertyChangedBase, ISettingsTabViewModel
    {
        public int Order { get; }

        public string DisplayName { get; }

        // This is purely for binding purposes
        public Type Type => GetType();

        public bool IsActive { get; set; }

        protected SettingsTabViewModelBase(int order, string displayName)
        {
            Order = order;
            DisplayName = displayName;
        }
    }
}