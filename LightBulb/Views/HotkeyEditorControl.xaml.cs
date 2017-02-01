using System.Windows;
using System.Windows.Input;
using LightBulb.Models;
using Tyrrrz.Extensions;

namespace LightBulb.Views
{
    public partial class HotkeyEditorControl
    {
        public static readonly DependencyProperty HotkeyProperty = DependencyProperty.Register(nameof(Models.Hotkey),
            typeof(Hotkey), typeof(HotkeyEditorControl),
            new FrameworkPropertyMetadata(default(Hotkey),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnHotkeyChanged));

        private static void OnHotkeyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var uc = (HotkeyEditorControl)sender;
            uc.UpdateText();
        }

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
            get { return (Hotkey) GetValue(HotkeyProperty); }
            set { SetValue(HotkeyProperty, value); }
        }

        public HotkeyEditorControl()
        {
            InitializeComponent();
            UpdateText();
        }

        private void HotkeyTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            var modifiers = Keyboard.Modifiers;
            var key = e.Key;
            if (key == Key.System) key = e.SystemKey; // wtf

            // If no actual key was pressed - return
            if (key.IsEither(
                Key.LeftCtrl, Key.RightCtrl, Key.LeftAlt, Key.RightAlt,
                Key.LeftShift, Key.RightShift, Key.LWin, Key.RWin,
                Key.Clear, Key.OemClear, Key.Apps))
            {
                return;
            }

            // Pressing delete, backspace or escape without modifiers clears the current value
            if (modifiers == ModifierKeys.None && key.IsEither(Key.Delete, Key.Back, Key.Escape))
            {
                Hotkey = Hotkey.Unset;
                return;
            }

            // Character keys cannot be used without modifier or with shift
            if (modifiers.IsEither(ModifierKeys.None, ModifierKeys.Shift) && HasKeyChar(key))
            {
                return;
            }

            // Set values
            Hotkey = new Hotkey(key, modifiers);
        }

        private void UpdateText()
        {
            HotkeyTextBox.Text = Hotkey.ToString();
        }
    }
}