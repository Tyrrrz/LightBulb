using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using LightBulb.Services;
using LightBulb.ViewModels;
using Microsoft.Practices.ServiceLocation;

namespace LightBulb
{
    public sealed class Locator
    {
        public static T Resolve<T>() => ServiceLocator.Current.GetInstance<T>();
        public static T Resolve<T>(string id) => ServiceLocator.Current.GetInstance<T>(id);

        static Locator()
        {
            if (ViewModelBase.IsInDesignModeStatic) return;

            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<TemperatureService>();
            SimpleIoc.Default.Register<GammaControlService>();
            SimpleIoc.Default.Register<WindowService>();
            SimpleIoc.Default.Register<GeolocationApiService>();

            SimpleIoc.Default.Register<MainViewModel>();
        }

        public static void Cleanup()
        {
            Resolve<GammaControlService>().RestoreDefault();
            Resolve<WindowService>().Dispose();
            Resolve<MainViewModel>().Dispose();
        }

        public MainViewModel Main => Resolve<MainViewModel>();
    }
}