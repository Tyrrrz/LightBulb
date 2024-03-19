using System;
using System.Threading;
using System.Threading.Tasks;
using DialogHostAvalonia;

namespace LightBulb.ViewModels.Framework;

public class DialogManager(IViewManager viewManager) : IDisposable
{
    private readonly SemaphoreSlim _dialogLock = new(1, 1);

    public async Task<T?> ShowDialogAsync<T>(DialogViewModel<T> dialogViewModel)
    {
        var view = GetViewForDialogScreen(dialogViewModel);

        void OnDialogOpened(object? openSender, DialogOpenedEventArgs openArgs)
        {
            void OnScreenClosed(object? closeSender, EventArgs args)
            {
                try
                {
                    openArgs.Session.Close();
                }
                catch (InvalidOperationException)
                {
                    // Race condition: dialog is already being closed
                }

                dialogViewModel.Closed -= OnScreenClosed;
            }

            dialogViewModel.Closed += OnScreenClosed;
        }

        await _dialogLock.WaitAsync();
        try
        {
            await DialogHost.Show(view, OnDialogOpened);
            return dialogViewModel.DialogResult;
        }
        finally
        {
            _dialogLock.Release();
        }
    }

    public void Dispose() => _dialogLock.Dispose();
}
