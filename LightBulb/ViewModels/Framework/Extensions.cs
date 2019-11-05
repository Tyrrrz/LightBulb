using LightBulb.ViewModels.Dialogs;
using Tyrrrz.Extensions;

namespace LightBulb.ViewModels.Framework
{
    public static class Extensions
    {
        public static MessageBoxViewModel CreateMessageBoxViewModel(this IViewModelFactory factory,
            string title, string message,
            string okButtonText, string cancelButtonText)
        {
            var viewModel = factory.CreateMessageBoxViewModel();
            viewModel.Title = title;
            viewModel.Message = message;

            viewModel.IsOkButtonVisible = !okButtonText.IsNullOrWhiteSpace();
            viewModel.OkButtonText = okButtonText;
            viewModel.IsCancelButtonVisible = !cancelButtonText.IsNullOrWhiteSpace();
            viewModel.CancelButtonText = cancelButtonText;

            return viewModel;
        }
    }
}