using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace LightBulb.Behaviors;

public class LostFocusUpdateBindingBehavior : Behavior<TextBox>
{
    public static readonly StyledProperty<string?> TextProperty = AvaloniaProperty.Register<
        LostFocusUpdateBindingBehavior,
        string?
    >(nameof(Text), defaultBindingMode: BindingMode.TwoWay);

    static LostFocusUpdateBindingBehavior()
    {
        TextProperty.Changed.Subscribe(args =>
        {
            if (args.Sender is LostFocusUpdateBindingBehavior behavior)
                behavior.OnBindingValueChanged();
        });
    }

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    protected override void OnAttached()
    {
        if (AssociatedObject is null)
            return;

        AssociatedObject.LostFocus += OnLostFocus;
        base.OnAttached();
    }

    protected override void OnDetaching()
    {
        if (AssociatedObject is null)
            return;

        AssociatedObject.LostFocus -= OnLostFocus;
        base.OnDetaching();
    }

    private void OnLostFocus(object? sender, RoutedEventArgs e)
    {
        if (AssociatedObject is null)
            return;

        Text = AssociatedObject.Text;
    }

    private void OnBindingValueChanged()
    {
        if (AssociatedObject != null)
            AssociatedObject.Text = Text;
    }
}
