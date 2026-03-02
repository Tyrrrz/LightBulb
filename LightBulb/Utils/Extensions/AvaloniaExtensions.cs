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

        public void Toggle()
        {
            if (window.IsVisible)
                window.Hide();
            else
                window.ShowActivateFocus();
        }

        public Task WaitUntilLoadedAsync()
        {
            var tcs = new TaskCompletionSource();

            void OnLoaded(object? _, RoutedEventArgs __)
            {
                window.Loaded -= OnLoaded;
                tcs.TrySetResult();
            }

            window.Loaded += OnLoaded;

            // Check after subscribing to avoid missing the event if the window
            // finishes loading between the check and the subscription
            if (window.IsLoaded)
                tcs.TrySetResult();

            return tcs.Task;
        }
    }
}
