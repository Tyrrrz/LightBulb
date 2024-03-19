using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LightBulb.ViewModels.Framework;

public abstract partial class DialogViewModel<T> : ObservableObject
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

public abstract class DialogViewModel : DialogViewModel<bool?>;
