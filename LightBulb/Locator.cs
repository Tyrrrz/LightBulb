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
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<WinApiService>();

            SimpleIoc.Default.Register<MainViewModel>();
        }

        public static void Cleanup()
        {
            Resolve<WinApiService>().RestoreOriginal();
        }

        public MainViewModel Main => Resolve<MainViewModel>();
    }
}