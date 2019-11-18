using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LightBulb.Models;
using Tyrrrz.Extensions;

namespace LightBulb.Views.Controls
{
    public class HotKeyTextBox : TextBox
    {
        public static readonly DependencyProperty HotKeyProperty =
            DependencyProperty.Register(nameof(HotKey), typeof(HotKey), typeof(HotKeyTextBox),
                new FrameworkPropertyMetadata(default(HotKey), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, HotKeyChanged));

        private static void HotKeyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is HotKeyTextBox control)
            {
                control.Text = control.HotKey.ToString();
            }
        }

        public HotKey HotKey
        {
            get => (HotKey) GetValue(HotKeyProperty);
            set => SetValue(HotKeyProperty, value);
        }

        public HotKeyTextBox()
        {
            IsReadOnly = true;
            IsReadOnlyCaretVisible = false;
            IsUndoEnabled = false;

            if (ContextMenu != null)
                ContextMenu.Visibility = Visibility.Collapsed;

            Text = HotKey.ToString();
        }

        private static bool HasKeyChar(Key key) =>
            // A - Z
            key >= Key.A && key <= Key.Z ||
            // 0 - 9
            key >= Key.D0 && key <= Key.D9 ||
            // Numpad 0 - 9
            key >= Key.NumPad0 && key <= Key.NumPad9 ||
            // The rest
            key.IsEither(
                Key.OemQuestion, Key.OemQuotes, Key.OemPlus, Key.OemOpenBrackets, Key.OemCloseBrackets,
                Key.OemMinus, Key.DeadCharProcessed, Key.Oem1, Key.Oem5, Key.Oem7, Key.OemPeriod, Key.OemComma, Key.Add,
                Key.Divide, Key.Multiply, Key.Subtract, Key.Oem102, Key.Decimal);

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            e.Handled = true;

            // Get modifiers and key data
            var modifiers = Keyboard.Modifiers;
            var key = e.Key;

            // If nothing was pressed - return
            if (key == Key.None)
                return;

            // If Alt is used as modifier - the key needs to be extracted from SystemKey
            if (key == Key.System)
                key = e.SystemKey;

            // If Delete/Backspace/Escape is pressed without modifiers - clear current value and return
            if (key.IsEither(Key.Delete, Key.Back, Key.Escape) && modifiers == ModifierKeys.None)
            {
                HotKey = HotKey.None;
                return;
            }

            // If the only key pressed is one of the modifier keys - return
            if (key.IsEither(
                Key.LeftCtrl, Key.RightCtrl, Key.LeftAlt, Key.RightAlt,
                Key.LeftShift, Key.RightShift, Key.LWin, Key.RWin,
                Key.Clear, Key.OemClear, Key.Apps))
                return;

            // If Enter/Space/Tab is pressed without modifiers - return
            if (key.IsEither(Key.Enter, Key.Space, Key.Tab) && modifiers == ModifierKeys.None)
                return;

            // If key has a character and pressed without modifiers or only with Shift - return
            if (HasKeyChar(key) && modifiers.IsEither(ModifierKeys.None, ModifierKeys.Shift))
                return;

            // Set value
            HotKey = new HotKey(key, modifiers);
        }
    }
}