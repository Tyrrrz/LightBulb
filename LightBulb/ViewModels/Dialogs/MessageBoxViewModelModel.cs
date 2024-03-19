using CommunityToolkit.Mvvm.ComponentModel;
using LightBulb.ViewModels.Framework;

namespace LightBulb.ViewModels.Dialogs;

public partial class MessageBoxViewModelModel : DialogViewModel
{
    [ObservableProperty]
    private string? _title;

    [ObservableProperty]
    private string? _message;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ButtonsCount))]
    private bool _isDefaultButtonVisible = true;

    [ObservableProperty]
    private string? _defaultButtonText;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ButtonsCount))]
    private bool _isCancelButtonVisible;

    [ObservableProperty]
    private string? _cancelButtonText;

    public int ButtonsCount => (IsDefaultButtonVisible ? 1 : 0) + (IsCancelButtonVisible ? 1 : 0);
}
