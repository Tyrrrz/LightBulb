using Avalonia;
using Avalonia.Controls;

namespace LightBulb.Views.Controls;

/// <summary>
/// Extends <see cref="TrayIcon"/> with a <see cref="DataContext"/> property,
/// allowing its properties to be set via Avalonia bindings.
/// </summary>
public class BindableTrayIcon : TrayIcon
{
    public static readonly StyledProperty<object?> DataContextProperty = AvaloniaProperty.Register<
        BindableTrayIcon,
        object?
    >(nameof(DataContext));

    /// <summary>
    /// Gets or sets an object that acts as the data context for bindings on this icon.
    /// </summary>
    public object? DataContext
    {
        get => GetValue(DataContextProperty);
        set => SetValue(DataContextProperty, value);
    }

    /// <summary>
    /// Registers this tray icon with the given <see cref="Application"/> so it is displayed.
    /// </summary>
    public void AttachToApplication(Application application) =>
        TrayIcon.SetIcons(application, new TrayIcons { this });
}
