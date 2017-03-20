using System;

namespace LightBulb.Models
{
    /// <summary>
    /// Handler for Windows hook events
    /// </summary>
    public delegate void WinEventHandler(
        IntPtr hWinEventHook, uint eventType, IntPtr hWnd,
        int idObject, int idChild, uint dwEventThread,
        uint dwmsEventTime);
}