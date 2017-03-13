using LightBulb.Models;

namespace LightBulb.Services.Interfaces
{
    /// <summary>
    /// Performs an action when a specific key sequence has been entered
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
        void RegisterHotkey(Hotkey hotkey, HotkeyHandler handler);

        /// <summary>
        /// Unregister a hotkey
        /// </summary>
        void UnregisterHotkey(Hotkey hotkey);

        /// <summary>
        /// Unregisters all hotkeys
        /// </summary>
        void UnregisterAllHotkeys();
    }
}