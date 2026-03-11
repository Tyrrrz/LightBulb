using Avalonia;
using Avalonia.Controls;

namespace LightBulb.Views.Controls;

/// <summary>
/// Extends <see cref="NativeMenuItem"/> with a <see cref="DataContext"/> property,
/// allowing its properties to be set via Avalonia bindings.
/// </summary>
public class BindableNativeMenuItem : NativeMenuItem
{
    public static readonly StyledProperty<object?> DataContextProperty = AvaloniaProperty.Register<
        BindableNativeMenuItem,
        object?
    >(nameof(DataContext));

    /// <summary>
    /// Gets or sets an object that acts as the data context for bindings on this item.
    /// </summary>
    public object? DataContext
    {
        get => GetValue(DataContextProperty);
        set => SetValue(DataContextProperty, value);
    }
}
