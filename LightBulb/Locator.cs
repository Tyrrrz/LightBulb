using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using LightBulb.Services;
using LightBulb.Services.Interfaces;
using LightBulb.ViewModels;
using Microsoft.Practices.ServiceLocation;

namespace LightBulb
{
    public sealed class Locator
    {
        static Locator()
        {
            if (ViewModelBase.IsInDesignModeStatic) return;

            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            // OS-specific
            SimpleIoc.Default.Register<IGammaService, WindowsGammaService>(true);
            SimpleIoc.Default.Register<IWindowService, WindowsWindowService>(true);

            // Neutral
            SimpleIoc.Default.Register<TemperatureService>(true);
            SimpleIoc.Default.Register<GeoSyncService>(true);

            // View models
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<GeneralSettingsViewModel>();
            SimpleIoc.Default.Register<GeoSettingsViewModel>();
            SimpleIoc.Default.Register<AdvancedSettingsViewModel>();
        }

        public static T Resolve<T>() => ServiceLocator.Current.GetInstance<T>();
        public static T Resolve<T>(string id) => ServiceLocator.Current.GetInstance<T>(id);

        public static void Cleanup()
        {
            (Resolve<IGammaService>() as IDisposable)?.Dispose();
            (Resolve<IWindowService>() as IDisposable)?.Dispose();
            Resolve<TemperatureService>().Dispose();
            Resolve<GeoSyncService>().Dispose();
            Resolve<MainViewModel>().Dispose();
        }

        public MainViewModel Main => Resolve<MainViewModel>();
        public GeneralSettingsViewModel GeneralSettings => Resolve<GeneralSettingsViewModel>();
        public GeoSettingsViewModel GeoSettings => Resolve<GeoSettingsViewModel>();
        public AdvancedSettingsViewModel AdvancedSettings => Resolve<AdvancedSettingsViewModel>();
    }
}