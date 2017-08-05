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
                (key >= Key.A && key <= Key.Z) ||
                // 0-9
                (key >= Key.D0 && key <= Key.D9) ||
                // Numpad 0-9
                (key >= Key.NumPad0 && key <= Key.NumPad9) ||
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
            {
                return;
            }

            // When Alt is pressed, SystemKey is used instead
            if (key == Key.System) key = e.SystemKey;

            // Pressing delete, backspace or escape without modifiers clears the current value
            if (modifiers == ModifierKeys.None && key.IsEither(Key.Delete, Key.Back, Key.Escape))
            {
                Hotkey = null;
                return;
            }

            // If no actual key was pressed - return
            if (key.IsEither(
                Key.LeftCtrl, Key.RightCtrl, Key.LeftAlt, Key.RightAlt,
                Key.LeftShift, Key.RightShift, Key.LWin, Key.RWin,
                Key.Clear, Key.OemClear, Key.Apps))
            {
                return;
            }

            // Character keys cannot be used without modifier or with shift
            if (modifiers.IsEither(ModifierKeys.None, ModifierKeys.Shift) && HasKeyChar(key))
            {
                return;
            }

            // Set values
            Hotkey = new Hotkey((int) key, (int) modifiers);
        }
    }
}