using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace LightBulb.Utils.Extensions;

internal static class AvaloniaExtensions
{
    extension(IApplicationLifetime lifetime)
    {
        public Window? TryGetMainWindow() =>
            lifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime
                ? desktopLifetime.MainWindow
                : null;

        public TopLevel? TryGetTopLevel() =>
            lifetime.TryGetMainWindow()
            ?? (lifetime as ISingleViewApplicationLifetime)?.MainView?.GetVisualRoot() as TopLevel;

        public bool TryShutdown(int exitCode = 0)
        {
            if (lifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                return desktopLifetime.TryShutdown(exitCode);
            }

            if (lifetime is IControlledApplicationLifetime controlledLifetime)
            {
                controlledLifetime.Shutdown(exitCode);
                return true;
            }

            return false;
        }
    }

    extension(Window window)
    {
        public void ShowActivateFocus()
        {
            window.Show();
            window.Activate();
            window.Focus();
        }

        public void Toggle()
        {
            if (window.IsVisible)
                window.Hide();
            else
                window.ShowActivateFocus();
        }

        public async Task WaitUntilLoadedAsync()
        {
            var tcs = new TaskCompletionSource();

            void OnLoaded(object? _, RoutedEventArgs __) => tcs.TrySetResult();
            void OnClosed(object? _, EventArgs __) => tcs.TrySetResult();

            window.Loaded += OnLoaded;
            window.Closed += OnClosed;

            if (window.IsLoaded)
                tcs.TrySetResult();

            await tcs.Task;
            window.Loaded -= OnLoaded;
            window.Closed -= OnClosed;
        }
    }
}
