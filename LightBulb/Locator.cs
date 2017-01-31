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

            // Services
            SimpleIoc.Default.Register<ISettingsService, FileSettingsService>();
            SimpleIoc.Default.Register<IGammaService, WindowsGammaService>();
            SimpleIoc.Default.Register<IWindowService, WindowsWindowService>();
            SimpleIoc.Default.Register<ITemperatureService, DefaultTemperatureService>();
            SimpleIoc.Default.Register<IGeoService, WebGeoService>();
            SimpleIoc.Default.Register<IVersionCheckService, GithubVersionCheckService>();

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
            (Resolve<ISettingsService>() as IDisposable)?.Dispose();
            (Resolve<IGammaService>() as IDisposable)?.Dispose();
            (Resolve<IWindowService>() as IDisposable)?.Dispose();
            (Resolve<ITemperatureService>() as IDisposable)?.Dispose();
            (Resolve<IGeoService>() as IDisposable)?.Dispose();
            (Resolve<IVersionCheckService>() as IDisposable)?.Dispose();

            Resolve<MainViewModel>().Dispose();
        }

        public MainViewModel Main => Resolve<MainViewModel>();
        public GeneralSettingsViewModel GeneralSettings => Resolve<GeneralSettingsViewModel>();
        public GeoSettingsViewModel GeoSettings => Resolve<GeoSettingsViewModel>();
        public AdvancedSettingsViewModel AdvancedSettings => Resolve<AdvancedSettingsViewModel>();
    }
}