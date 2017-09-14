using System.Windows;
using System.Windows.Input;
using LightBulb.Models;
using Tyrrrz.Extensions;

namespace LightBulb.Views
{
    public partial class HotkeyEditorControl
    {
        public static readonly DependencyProperty HotkeyProperty =
            DependencyProperty.Register(nameof(Hotkey), typeof(Hotkey), typeof(HotkeyEditorControl),
                new FrameworkPropertyMetadata(default(Hotkey), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private static bool HasKeyChar(Key key)
        {
            return
                // A-Z
                key >= Key.A && key <= Key.Z ||
                // 0-9
                key >= Key.D0 && key <= Key.D9 ||
                // Numpad 0-9
                key >= Key.NumPad0 && key <= Key.NumPad9 ||
                // The rest
                key.IsEither(
                    Key.OemQuestion, Key.OemQuotes, Key.OemPlus, Key.OemOpenBrackets, Key.OemCloseBrackets,
                    Key.OemMinus, Key.DeadCharProcessed, Key.Oem1, Key.Oem5, Key.Oem7, Key.OemPeriod, Key.OemComma, Key.Add,
                    Key.Divide, Key.Multiply, Key.Subtract, Key.Oem102, Key.Decimal);
        }

        /// <summary>
        /// Currently selected hotkey
        /// </summary>
        public Hotkey Hotkey
        {
            get => (Hotkey) GetValue(HotkeyProperty);
            set => SetValue(HotkeyProperty, value);
        }

        public HotkeyEditorControl()
        {
            InitializeComponent();
        }

        private void HotkeyTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Don't let the event pass further, because we don't want standard textbox shortcuts working
            e.Handled = true;

            // Get modifiers and key data
            var modifiers = Keyboard.Modifiers;
            var key = e.Key;

            // Nothing pressed - return
            if (key == Key.None)
                return;

            // If Alt is used as modifier - extract key from SystemKey instead
            if (key == Key.System)
                key = e.SystemKey;

            // If Delete/Backspace/Escape is pressed without modifiers - clear current hotkey
            if (key.IsEither(Key.Delete, Key.Back, Key.Escape) && modifiers == ModifierKeys.None)
            {
                Hotkey = null;
                return;
            }

            // If no actual key was pressed - return
            if (key.IsEither(
                Key.LeftCtrl, Key.RightCtrl, Key.LeftAlt, Key.RightAlt,
                Key.LeftShift, Key.RightShift, Key.LWin, Key.RWin,
                Key.Clear, Key.OemClear, Key.Apps))
                return;

            // If character keys are pressed with Shift or without modifiers - return
            if (HasKeyChar(key) && modifiers.IsEither(ModifierKeys.None, ModifierKeys.Shift))
                return;

            // If Enter/Space/Tab is pressed without modifiers - return
            if (key.IsEither(Key.Enter, Key.Space, Key.Tab) && modifiers == ModifierKeys.None)
                return;

            // Set values
            Hotkey = new Hotkey((int) key, (int) modifiers);
        }
    }
}