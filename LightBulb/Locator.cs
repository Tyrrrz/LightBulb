using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using LightBulb.Services;
using LightBulb.Services.Interfaces;
using LightBulb.ViewModels;
using LightBulb.ViewModels.Interfaces;
using Microsoft.Practices.ServiceLocation;

namespace LightBulb
{
    public sealed class Locator
    {
        private static bool _isInit;

        public static T Resolve<T>() => ServiceLocator.Current.GetInstance<T>();
        public static T Resolve<T>(string id) => ServiceLocator.Current.GetInstance<T>(id);

        /// <summary>
        /// Initialize service locator
        /// </summary>
        public static void Init()
        {
            if (_isInit) return;
            if (ViewModelBase.IsInDesignModeStatic) return;

            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            // Services
            SimpleIoc.Default.Register<ISettingsService, FileSettingsService>();
            SimpleIoc.Default.Register<IGammaService, WindowsGammaService>();
            SimpleIoc.Default.Register<IWindowService, WindowsWindowService>();
            SimpleIoc.Default.Register<IHotkeyService, WindowsHotkeyService>();
            SimpleIoc.Default.Register<ITemperatureService, TemperatureService>();
            SimpleIoc.Default.Register<IGeoService, WebGeoService>();
            SimpleIoc.Default.Register<IVersionCheckService, GithubVersionCheckService>();

            // View models
            SimpleIoc.Default.Register<IMainViewModel, MainViewModel>();
            SimpleIoc.Default.Register<IGeneralSettingsViewModel, GeneralSettingsViewModel>();
            SimpleIoc.Default.Register<IGeoSettingsViewModel, GeoSettingsViewModel>();
            SimpleIoc.Default.Register<IAdvancedSettingsViewModel, AdvancedSettingsViewModel>();

            _isInit = true;
        }

        /// <summary>
        /// Cleanup resources used by service locator
        /// </summary>
        public static void Cleanup()
        {
            if (!_isInit) return;

            // ReSharper disable SuspiciousTypeConversion.Global
            (Resolve<ISettingsService>() as IDisposable)?.Dispose();
            (Resolve<IGammaService>() as IDisposable)?.Dispose();
            (Resolve<IWindowService>() as IDisposable)?.Dispose();
            (Resolve<IHotkeyService>() as IDisposable)?.Dispose();
            (Resolve<ITemperatureService>() as IDisposable)?.Dispose();
            (Resolve<IGeoService>() as IDisposable)?.Dispose();
            (Resolve<IVersionCheckService>() as IDisposable)?.Dispose();

            (Resolve<IMainViewModel>() as IDisposable)?.Dispose();
            (Resolve<IGeneralSettingsViewModel>() as IDisposable)?.Dispose();
            (Resolve<IGeoSettingsViewModel>() as IDisposable)?.Dispose();
            (Resolve<IAdvancedSettingsViewModel>() as IDisposable)?.Dispose();
            // ReSharper restore SuspiciousTypeConversion.Global
        }

        public IMainViewModel Main => Resolve<IMainViewModel>();
        public IGeneralSettingsViewModel GeneralSettings => Resolve<IGeneralSettingsViewModel>();
        public IGeoSettingsViewModel GeoSettings => Resolve<IGeoSettingsViewModel>();
        public IAdvancedSettingsViewModel AdvancedSettings => Resolve<IAdvancedSettingsViewModel>();
    }
}