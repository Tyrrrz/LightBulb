using LightBulb.Models;
using LightBulb.ViewModels.Components;

namespace LightBulb.ViewModels.Framework
{
    public static class Extensions
    {
        public static HotKeyViewModel CreateHotKeyViewModel(this IViewModelFactory factory, HotKey model)
        {
            var viewModel = factory.CreateHotKeyViewModel();
            viewModel.Model = model;

            return viewModel;
        }
    }
}