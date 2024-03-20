using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LightBulb.Framework;

public abstract partial class DialogViewModelBase<T> : ViewModelBase
{
    [ObservableProperty]
    private T? _dialogResult;

    public event EventHandler? Closed;

    [RelayCommand]
    protected void Close(T dialogResult)
    {
        DialogResult = dialogResult;
        Closed?.Invoke(this, EventArgs.Empty);
    }
}

public abstract class DialogViewModelBase : DialogViewModelBase<bool?>;
