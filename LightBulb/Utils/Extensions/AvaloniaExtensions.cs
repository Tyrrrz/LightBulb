using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;

namespace LightBulb.Utils.Extensions;

internal static class AvaloniaExtensions
{
    extension(IApplicationLifetime lifetime)
    {
        public Window? TryGetMainWindow() =>
            lifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime
                ? desktopLifetime.MainWindow
                : null;

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

        public async Task WaitUntilLoadedAsync(CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

            await using var registration = cancellationToken.Register(() =>
                tcs.TrySetCanceled(cancellationToken)
            );

            void OnLoaded(object? _, RoutedEventArgs __) => tcs.TrySetResult();
            window.Loaded += OnLoaded;

            void OnClosed(object? _, EventArgs __) => tcs.TrySetCanceled();
            window.Closed += OnClosed;

            if (window.IsLoaded)
                tcs.TrySetResult();

            try
            {
                await tcs.Task;
            }
            finally
            {
                window.Loaded -= OnLoaded;
                window.Closed -= OnClosed;
            }
        }
    }
}
