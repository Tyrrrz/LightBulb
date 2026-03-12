using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Metadata;

namespace LightBulb.Views.Controls;

/// <summary>
/// A <see cref="StyledElement"/>-based wrapper for <see cref="NativeMenuItem"/>.
/// Exposes typed Avalonia styled properties that accept compiled XAML bindings;
/// each property change is forwarded to the inner <see cref="NativeMenuItem"/>
/// so the native tray menu always reflects the current values.
/// </summary>
public class BindableNativeMenuItem : StyledElement
{
    public static readonly StyledProperty<string?> HeaderProperty = AvaloniaProperty.Register<
        BindableNativeMenuItem,
        string?
    >(nameof(Header));

    public static readonly StyledProperty<ICommand?> CommandProperty = AvaloniaProperty.Register<
        BindableNativeMenuItem,
        ICommand?
    >(nameof(Command));

    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<BindableNativeMenuItem, object?>(nameof(CommandParameter));

    internal NativeMenuItem NativeMenuItem { get; } = new();

    static BindableNativeMenuItem()
    {
        HeaderProperty.Changed.AddClassHandler<BindableNativeMenuItem>(
            (x, _) => x.NativeMenuItem.Header = x.Header ?? string.Empty
        );
        CommandProperty.Changed.AddClassHandler<BindableNativeMenuItem>(
            (x, _) => x.NativeMenuItem.Command = x.Command
        );
        CommandParameterProperty.Changed.AddClassHandler<BindableNativeMenuItem>(
            (x, _) => x.NativeMenuItem.CommandParameter = x.CommandParameter
        );
    }

    public BindableNativeMenuItem()
    {
        Items.CollectionChanged += OnItemsChanged;
    }

    public string? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    [Content]
    public AvaloniaList<object> Items { get; } = new AvaloniaList<object>();

    private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // Make BindableNativeMenuItems logical children so they inherit DataContext.
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

        RebuildSubMenu();
    }

    private void RebuildSubMenu()
    {
        if (Items.Count == 0)
        {
            NativeMenuItem.Menu = null;
            return;
        }

        var menu = new NativeMenu();
        foreach (var item in Items)
        {
            if (item is BindableNativeMenuItem child)
                menu.Items.Add(child.NativeMenuItem);
            else if (item is NativeMenuItemSeparator sep)
                menu.Items.Add(sep);
        }

        NativeMenuItem.Menu = menu;
    }
}
