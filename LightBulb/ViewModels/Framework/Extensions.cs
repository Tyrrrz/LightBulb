using LightBulb.ViewModels.Dialogs;

namespace LightBulb.ViewModels.Framework
{
    public static class Extensions
    {
        public static MessageBoxViewModel CreateMessageBoxViewModel(this IViewModelFactory factory,
            string? title, string? message,
            string? okButtonText, string? cancelButtonText)
        {
            var viewModel = factory.CreateMessageBoxViewModel();
            viewModel.Title = title;
            viewModel.Message = message;

            viewModel.IsOkButtonVisible = !string.IsNullOrWhiteSpace(okButtonText);
            viewModel.OkButtonText = okButtonText;
            viewModel.IsCancelButtonVisible = !string.IsNullOrWhiteSpace(okButtonText);
            viewModel.CancelButtonText = cancelButtonText;

            return viewModel;
        }
    }
}