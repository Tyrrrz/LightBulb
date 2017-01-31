using System;

namespace LightBulb.Services.Interfaces
{
    /// <summary>
    /// Manages display gamma via time-based color temperature profile
    /// </summary>
    public interface ITemperatureService
    {
        /// <summary>
        /// Current display color temperature
        /// </summary>
        ushort Temperature { get; }

        /// <summary>
        /// Whether realtime temperature control mode is enabled
        /// </summary>
        bool IsRealtimeModeEnabled { get; set; }

        /// <summary>
        /// Whether temperature preview mode is enabled
        /// </summary>
        bool IsPreviewModeEnabled { get; set; }

        /// <summary>
        /// Current cycle preview time
        /// </summary>
        DateTime CyclePreviewTime { get; }

        /// <summary>
        /// Whether 24hr cycle preview is currently in progress
        /// </summary>
        bool IsCyclePreviewRunning { get; }

        /// <summary>
        /// Triggered when something updates
        /// </summary>
        event EventHandler Updated;

        /// <summary>
        /// Request temperature for preview mode
        /// </summary>
        void RequestPreviewTemperature(ushort temp);

        /// <summary>
        /// Start 24hr cycle preview
        /// </summary>
        void StartCyclePreview();
    }
}