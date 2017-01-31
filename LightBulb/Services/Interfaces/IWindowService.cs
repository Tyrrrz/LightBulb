using System;

namespace LightBulb.Services.Interfaces
{
    /// <summary>
    /// Implemented by wrappers that can query the state of specifci windows
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