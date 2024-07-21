using CommunityToolkit.Mvvm.ComponentModel;
using LightBulb.Framework;

namespace LightBulb.ViewModels.Dialogs;

public partial class MessageBoxViewModel : DialogViewModelBase
{
    [ObservableProperty]
    private string? _title = "Title";

    [ObservableProperty]
    private string? _message = "Message";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDefaultButtonVisible))]
    [NotifyPropertyChangedFor(nameof(ButtonsCount))]
    private string? _defaultButtonText = "OK";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsCancelButtonVisible))]
    [NotifyPropertyChangedFor(nameof(ButtonsCount))]
    private string? _cancelButtonText = "Cancel";

    private bool _isCheckboxVisible;
    public bool IsCheckboxVisible
    {
        get => _isCheckboxVisible;
        set => SetProperty(ref _isCheckboxVisible, value);
    }

    private bool _isCheckboxChecked;
    public bool IsCheckboxChecked
    {
        get => _isCheckboxChecked;
        set => SetProperty(ref _isCheckboxChecked, value);
    }

    public bool IsDefaultButtonVisible => !string.IsNullOrWhiteSpace(DefaultButtonText);

    public bool IsCancelButtonVisible => !string.IsNullOrWhiteSpace(CancelButtonText);

    public int ButtonsCount => (IsDefaultButtonVisible ? 1 : 0) + (IsCancelButtonVisible ? 1 : 0);
}
