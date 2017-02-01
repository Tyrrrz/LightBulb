using System.Windows.Input;

namespace LightBulb.Services.Interfaces
{
    /// <summary>
    /// Handles hotkeys
    /// </summary>
    public delegate void HotkeyHandler();

    /// <summary>
    /// Implemented by classes that handle hotkeys
    /// </summary>
    public interface IHotkeyService
    {
        /// <summary>
        /// Register a hotkey to be handled by the given delegate
        /// </summary>
        void Register(Key key, ModifierKeys modifiers, HotkeyHandler handler);

        /// <summary>
        /// Unregisters all hotkeys
        /// </summary>
        void UnregisterAll();
    }
}