﻿using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LightBulb.Framework;

public abstract partial class DialogViewModelBase<T> : ViewModelBase
{
    private readonly TaskCompletionSource<T> _closeTcs =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    [ObservableProperty]
    private T? _dialogResult;

    [RelayCommand]
    protected void Close(T dialogResult)
    {
        DialogResult = dialogResult;
        _closeTcs.TrySetResult(dialogResult);
    }

    public async Task<T> WaitForCloseAsync() => await _closeTcs.Task;
}

public abstract class DialogViewModelBase : DialogViewModelBase<bool?>;
