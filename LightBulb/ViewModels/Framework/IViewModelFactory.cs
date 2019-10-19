﻿using LightBulb.ViewModels.Components;
using LightBulb.ViewModels.Dialogs;

namespace LightBulb.ViewModels.Framework
{
    // Used to instantiate new view models while making use of dependency injection
    public interface IViewModelFactory
    {
        HotKeyViewModel CreateHotKeyViewModel();

        MessageBoxViewModel CreateMessageBoxViewModel();

        SettingsViewModel CreateSettingsViewModel();
    }
}