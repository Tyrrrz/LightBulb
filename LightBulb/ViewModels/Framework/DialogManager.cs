using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using MaterialDesignThemes.Wpf;
using Stylet;

namespace LightBulb.ViewModels.Framework;

public class DialogManager(IViewManager viewManager) : IDisposable
{
    // Cache and reuse dialog screen views, as creating them is incredibly slow
    private readonly Dictionary<Type, UIElement> _dialogScreenViewCache = new();
    private readonly SemaphoreSlim _dialogLock = new(1, 1);

    public UIElement GetViewForDialogScreen<T>(DialogScreen<T> dialogScreen)
    {
        var dialogScreenType = dialogScreen.GetType();

        if (_dialogScreenViewCache.TryGetValue(dialogScreenType, out var cachedView))
        {
            viewManager.BindViewToModel(cachedView, dialogScreen);
            return cachedView;
        }
        else
        {
            var view = viewManager.CreateAndBindViewForModelIfNecessary(dialogScreen);

            // This warms up the view and triggers all bindings.
            // We need to do this, as the view may have nested model-bound ContentControls
            // which take a very long time to load.
            // By pre-loading them as early as possible, we avoid doing it when the dialog
            // actually pops up, which improves user experience.
            // Ideally, the whole view cache should be populated at application startup.
            view.Arrange(new Rect(0, 0, 500, 500));

            return _dialogScreenViewCache[dialogScreenType] = view;
        }
    }

    public async Task<T?> ShowDialogAsync<T>(DialogScreen<T> dialogScreen)
    {
        var view = GetViewForDialogScreen(dialogScreen);

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

                dialogScreen.Closed -= OnScreenClosed;
            }

            dialogScreen.Closed += OnScreenClosed;
        }

        await _dialogLock.WaitAsync();
        try
        {
            await DialogHost.Show(view, OnDialogOpened);
            return dialogScreen.DialogResult;
        }
        finally
        {
            _dialogLock.Release();
        }
    }

    public void Dispose()
    {
        _dialogLock.Dispose();
    }
}
