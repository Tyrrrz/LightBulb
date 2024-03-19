using System;
using System.Threading;
using System.Threading.Tasks;
using DialogHostAvalonia;
using LightBulb.Views.Framework;

namespace LightBulb.ViewModels.Framework;

public class DialogManager(ViewLocator viewLocator) : IDisposable
{
    private readonly SemaphoreSlim _dialogLock = new(1, 1);

    public async Task<T?> ShowDialogAsync<T>(DialogViewModelBase<T> dialog)
    {
        var view = viewLocator.TryResolveView(dialog);
        if (view is null)
        {
            throw new InvalidOperationException(
                $"View not found for dialog view model '{dialog.GetType()}'."
            );
        }

        void OnDialogOpened(object? openSender, DialogOpenedEventArgs openArgs)
        {
            void OnDialogClosed(object? closeSender, EventArgs args)
            {
                try
                {
                    openArgs.Session.Close();
                }
                catch (InvalidOperationException)
                {
                    // Race condition: dialog is already being closed
                }

                dialog.Closed -= OnDialogClosed;
            }

            dialog.Closed += OnDialogClosed;
        }

        await _dialogLock.WaitAsync();
        try
        {
            await DialogHost.Show(view, OnDialogOpened);
            return dialog.DialogResult;
        }
        finally
        {
            _dialogLock.Release();
        }
    }

    public void Dispose() => _dialogLock.Dispose();
}
