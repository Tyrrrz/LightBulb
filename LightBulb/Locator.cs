using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using LightBulb.Services;
using LightBulb.ViewModels;
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
            SimpleIoc.Default.Register<IGammaService, WindowsGammaService>();
            SimpleIoc.Default.Register<IGeoService, WebGeoService>();
            SimpleIoc.Default.Register<IHotkeyService, WindowsHotkeyService>();
            SimpleIoc.Default.Register<IHttpService, HttpService>();
            SimpleIoc.Default.Register<ISettingsService, FileSettingsService>();
            SimpleIoc.Default.Register<ITemperatureService, TemperatureService>();
            SimpleIoc.Default.Register<IVersionCheckService, WebVersionCheckService>();
            SimpleIoc.Default.Register<IWindowService, WindowsWindowService>();

            // View models
            SimpleIoc.Default.Register<IAdvancedSettingsViewModel, AdvancedSettingsViewModel>();
            SimpleIoc.Default.Register<IGeneralSettingsViewModel, GeneralSettingsViewModel>();
            SimpleIoc.Default.Register<IGeoSettingsViewModel, GeoSettingsViewModel>();
            SimpleIoc.Default.Register<IMainViewModel, MainViewModel>();

            // Load settings
            Resolve<ISettingsService>().Load();

            _isInit = true;
        }

        /// <summary>
        /// Cleanup resources used by service locator
        /// </summary>
        public static void Cleanup()
        {
            if (!_isInit) return;

            // Save settings
            Resolve<ISettingsService>().Save();

            // ReSharper disable SuspiciousTypeConversion.Global
            (Resolve<IGammaService>() as IDisposable)?.Dispose();
            (Resolve<IGeoService>() as IDisposable)?.Dispose();
            (Resolve<IHotkeyService>() as IDisposable)?.Dispose();
            (Resolve<IHttpService>() as IDisposable)?.Dispose();
            (Resolve<ISettingsService>() as IDisposable)?.Dispose();
            (Resolve<ITemperatureService>() as IDisposable)?.Dispose();
            (Resolve<IVersionCheckService>() as IDisposable)?.Dispose();
            (Resolve<IWindowService>() as IDisposable)?.Dispose();

            (Resolve<IAdvancedSettingsViewModel>() as IDisposable)?.Dispose();
            (Resolve<IGeneralSettingsViewModel>() as IDisposable)?.Dispose();
            (Resolve<IGeoSettingsViewModel>() as IDisposable)?.Dispose();
            (Resolve<IMainViewModel>() as IDisposable)?.Dispose();
            // ReSharper restore SuspiciousTypeConversion.Global
        }

        public IAdvancedSettingsViewModel AdvancedSettingsViewModel => Resolve<IAdvancedSettingsViewModel>();
        public IGeneralSettingsViewModel GeneralSettingsViewModel => Resolve<IGeneralSettingsViewModel>();
        public IGeoSettingsViewModel GeoSettingsViewModel => Resolve<IGeoSettingsViewModel>();
        public IMainViewModel MainViewModel => Resolve<IMainViewModel>();
    }
}