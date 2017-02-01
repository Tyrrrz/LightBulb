using System;

namespace LightBulb.Services.Interfaces
{
    /// <summary>
    /// Implemented by classes that can query the state of specific windows
    /// </summary>
    public interface IWindowService
    {
        /// <summary>
        /// Whether the foreground window is currently fullscreen
        /// </summary>
        bool IsForegroundFullScreen { get; }

        /// <summary>
        /// Triggers when the foreground window has entered (or exited from) full screen mode
        /// </summary>
        event EventHandler FullScreenStateChanged;
    }
}