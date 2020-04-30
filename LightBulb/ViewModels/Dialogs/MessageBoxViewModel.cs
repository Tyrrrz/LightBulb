using LightBulb.ViewModels.Framework;

namespace LightBulb.ViewModels.Dialogs
{
    public class MessageBoxViewModel : DialogScreen
    {
        public string? Title { get; set; }

        public string? Message { get; set; }

        public bool IsOkButtonVisible { get; set; } = true;

        public string? OkButtonText { get; set; } = "OK";

        public bool IsCancelButtonVisible { get; set; } = false;

        public string? CancelButtonText { get; set; } = "CANCEL";
    }

    public static class MessageBoxViewModelExtensions
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
            viewModel.IsCancelButtonVisible = !string.IsNullOrWhiteSpace(cancelButtonText);
            viewModel.CancelButtonText = cancelButtonText;

            return viewModel;
        }
    }
}