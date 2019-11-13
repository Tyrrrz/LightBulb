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
}