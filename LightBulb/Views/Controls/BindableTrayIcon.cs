using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Metadata;

namespace LightBulb.Views.Controls;

/// <summary>
/// A <see cref="StyledElement"/>-based wrapper for <see cref="TrayIcon"/>.
/// Exposes typed Avalonia styled properties that accept compiled XAML bindings;
/// each property change is forwarded to the inner <see cref="TrayIcon"/> so the
/// native tray icon always reflects the current values.
/// Call <see cref="AttachToApplication"/> after setting
/// <see cref="StyledElement.DataContext"/> to register the icon with the application.
/// </summary>
public class BindableTrayIcon : StyledElement
{
    public static readonly StyledProperty<string?> ToolTipTextProperty = AvaloniaProperty.Register<
        BindableTrayIcon,
        string?
    >(nameof(ToolTipText));

    public static readonly StyledProperty<ICommand?> CommandProperty = AvaloniaProperty.Register<
        BindableTrayIcon,
        ICommand?
    >(nameof(Command));

    public static readonly StyledProperty<WindowIcon?> IconProperty = AvaloniaProperty.Register<
        BindableTrayIcon,
        WindowIcon?
    >(nameof(Icon));

    private readonly TrayIcon _trayIcon = new();

    static BindableTrayIcon()
    {
        ToolTipTextProperty.Changed.AddClassHandler<BindableTrayIcon>(
            (x, _) => x._trayIcon.ToolTipText = x.ToolTipText ?? string.Empty
        );
        CommandProperty.Changed.AddClassHandler<BindableTrayIcon>(
            (x, _) => x._trayIcon.Command = x.Command
        );
        IconProperty.Changed.AddClassHandler<BindableTrayIcon>((x, _) => x._trayIcon.Icon = x.Icon);
    }

    public BindableTrayIcon()
    {
        Items.CollectionChanged += OnItemsChanged;
    }

    public string? ToolTipText
    {
        get => GetValue(ToolTipTextProperty);
        set => SetValue(ToolTipTextProperty, value);
    }

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public WindowIcon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    [Content]
    public AvaloniaList<object> Items { get; } = new AvaloniaList<object>();

    private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
        {
            foreach (var item in e.OldItems.OfType<BindableNativeMenuItem>())
                LogicalChildren.Remove(item);
        }

        if (e.NewItems is not null)
        {
            foreach (var item in e.NewItems.OfType<BindableNativeMenuItem>())
                LogicalChildren.Add(item);
        }

        RebuildMenu();
    }

    private void RebuildMenu()
    {
        var menu = new NativeMenu();
        foreach (var item in Items)
        {
            if (item is BindableNativeMenuItem menuItem)
                menu.Items.Add(menuItem.NativeMenuItem);
            else if (item is NativeMenuItemSeparator sep)
                menu.Items.Add(sep);
        }

        _trayIcon.Menu = menu;
    }

    /// <summary>
    /// Registers this tray icon with the given application so it is displayed.
    /// </summary>
    public void AttachToApplication(Application application) =>
        TrayIcon.SetIcons(application, new TrayIcons { _trayIcon });
}
