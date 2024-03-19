using CommunityToolkit.Mvvm.ComponentModel;
using LightBulb.ViewModels.Framework;

namespace LightBulb.ViewModels.Dialogs;

public partial class MessageBoxViewModel : DialogViewModel
{
    [ObservableProperty]
    private string? _title = "Title";

    [ObservableProperty]
    private string? _message = "Message";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ButtonsCount))]
    private bool _isDefaultButtonVisible = true;

    [ObservableProperty]
    private string? _defaultButtonText = "OK";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ButtonsCount))]
    private bool _isCancelButtonVisible = true;

    [ObservableProperty]
    private string? _cancelButtonText = "Cancel";

    public int ButtonsCount => (IsDefaultButtonVisible ? 1 : 0) + (IsCancelButtonVisible ? 1 : 0);
}
