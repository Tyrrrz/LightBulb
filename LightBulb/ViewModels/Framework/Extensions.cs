using LightBulb.ViewModels.Dialogs;

namespace LightBulb.ViewModels.Framework
{
    public static class Extensions
    {
        public static MessageBoxViewModel CreateMessageBoxViewModel(this IViewModelFactory factory, string title,
            string message)
        {
            var viewModel = factory.CreateMessageBoxViewModel();
            viewModel.Title = title;
            viewModel.Message = message;

            return viewModel;
        }
    }
}