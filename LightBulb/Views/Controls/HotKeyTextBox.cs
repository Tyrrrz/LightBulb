using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using LightBulb.Models;

namespace LightBulb.Views.Controls;

public class HotKeyTextBox : TextBox
{
    public static readonly StyledProperty<HotKey> HotKeyProperty = AvaloniaProperty.Register<
        HotKeyTextBox,
        HotKey
    >(nameof(HotKey), HotKey.None, false, BindingMode.TwoWay);

    public HotKey HotKey
    {
        get => GetValue(HotKeyProperty);
        set => SetValue(HotKeyProperty, value);
    }

    public HotKeyTextBox()
    {
        IsReadOnly = true;
        IsUndoEnabled = false;

        Text = HotKey.ToString();
    }

    protected override void OnKeyDown(KeyEventArgs args)
    {
        args.Handled = true;

        var modifiers = args.KeyModifiers;
        var key = args.PhysicalKey;

        if (key == PhysicalKey.None)
            return;

        // Clear the current value if Delete/Back/Escape is pressed without modifiers
        if (
            key is PhysicalKey.Delete or PhysicalKey.Backspace or PhysicalKey.Escape
            && modifiers == KeyModifiers.None
        )
        {
            HotKey = HotKey.None;
            return;
        }

        // Require at least one non-modifier key to be pressed
        if (
            key
            is PhysicalKey.ControlLeft
                or PhysicalKey.ControlRight
                or PhysicalKey.AltLeft
                or PhysicalKey.AltRight
                or PhysicalKey.ShiftLeft
                or PhysicalKey.ShiftRight
                or PhysicalKey.NumPadClear
        )
        {
            return;
        }

        // Don't allow Enter/Space/Tab to be used as hotkeys without modifiers
        if (
            key is PhysicalKey.Enter or PhysicalKey.Space or PhysicalKey.Tab
            && modifiers == KeyModifiers.None
        )
        {
            return;
        }

        // Don't allow character keys to be used as hotkeys without modifiers or with Shift
        if (
            key.ToQwertyKeySymbol() is not null
            && (modifiers == KeyModifiers.None || modifiers.HasFlag(KeyModifiers.Shift))
        )
        {
            return;
        }

        // Set value
        HotKey = new HotKey(key, modifiers);
    }
}
