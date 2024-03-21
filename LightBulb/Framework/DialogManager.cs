using System;
using System.Threading;
using System.Threading.Tasks;
using DialogHostAvalonia;

namespace LightBulb.Framework;

public class DialogManager(ViewBinder viewBinder) : IDisposable
{
    private readonly SemaphoreSlim _dialogLock = new(1, 1);

    public async Task<T?> ShowDialogAsync<T>(DialogViewModelBase<T> dialog)
    {
        var view = viewBinder.TryBindView(dialog);
        if (view is null)
        {
            throw new InvalidOperationException(
                $"View not found for dialog view model '{dialog.GetType()}'."
            );
        }

        await _dialogLock.WaitAsync();
        try
        {
            await DialogHost.Show(
                view,
                (object openSender, DialogOpenedEventArgs openArgs) =>
                {
                    void OnClosed(object? closedSender, EventArgs closedArgs)
                    {
                        try
                        {
                            openArgs.Session.Close();
                        }
                        catch (InvalidOperationException)
                        {
                            // Race condition: dialog is already being closed
                        }

                        dialog.Closed -= OnClosed;
                    }

                    dialog.Closed += OnClosed;
                }
            );

            return dialog.DialogResult;
        }
        finally
        {
            _dialogLock.Release();
        }
    }

    public void Dispose() => _dialogLock.Dispose();
}
