using LightBulb.ViewModels.Framework;

namespace LightBulb.ViewModels.Dialogs;

public class MessageBoxViewModelModel : DialogViewModel
{
    public string? Title { get; set; }

    public string? Message { get; set; }

    public bool IsDefaultButtonVisible { get; set; } = true;

    public string? DefaultButtonText { get; set; }

    public bool IsCancelButtonVisible { get; set; }

    public string? CancelButtonText { get; set; }

    public int ButtonsCount => (IsDefaultButtonVisible ? 1 : 0) + (IsCancelButtonVisible ? 1 : 0);
}
