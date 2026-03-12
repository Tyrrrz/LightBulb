using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Metadata;
using AvaloniaNativeMenuItem = Avalonia.Controls.NativeMenuItem;

namespace LightBulb.Views.Controls;

/// <summary>
/// A <see cref="StyledElement"/>-based wrapper for <see cref="AvaloniaNativeMenuItem"/>.
/// Exposes typed Avalonia styled properties that accept compiled XAML bindings;
/// each property change is forwarded to the inner <see cref="AvaloniaNativeMenuItem"/>
/// so the native tray menu always reflects the current values.
/// </summary>
public class NativeMenuItem : StyledElement
{
    public static readonly StyledProperty<string?> HeaderProperty = AvaloniaProperty.Register<
        NativeMenuItem,
        string?
    >(nameof(Header));

    public static readonly StyledProperty<ICommand?> CommandProperty = AvaloniaProperty.Register<
        NativeMenuItem,
        ICommand?
    >(nameof(Command));

    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<NativeMenuItem, object?>(nameof(CommandParameter));

    internal AvaloniaNativeMenuItem Inner { get; } = new();

    static NativeMenuItem()
    {
        HeaderProperty.Changed.AddClassHandler<NativeMenuItem>(
            (x, _) => x.Inner.Header = x.Header ?? string.Empty
        );
        CommandProperty.Changed.AddClassHandler<NativeMenuItem>(
            (x, _) => x.Inner.Command = x.Command
        );
        CommandParameterProperty.Changed.AddClassHandler<NativeMenuItem>(
            (x, _) => x.Inner.CommandParameter = x.CommandParameter
        );
    }

    public NativeMenuItem()
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
        // Make NativeMenuItems logical children so they inherit DataContext.
        if (e.OldItems is not null)
        {
            foreach (var item in e.OldItems.OfType<NativeMenuItem>())
                LogicalChildren.Remove(item);
        }

        if (e.NewItems is not null)
        {
            foreach (var item in e.NewItems.OfType<NativeMenuItem>())
                LogicalChildren.Add(item);
        }

        RebuildSubMenu();
    }

    private void RebuildSubMenu()
    {
        if (Items.Count == 0)
        {
            Inner.Menu = null;
            return;
        }

        var menu = new NativeMenu();
        foreach (var item in Items)
        {
            if (item is NativeMenuItem child)
                menu.Items.Add(child.Inner);
            else if (item is NativeMenuItemSeparator sep)
                menu.Items.Add(sep);
        }

        Inner.Menu = menu;
    }
}
