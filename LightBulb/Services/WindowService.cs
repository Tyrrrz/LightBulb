using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using LightBulb.Models;

// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable

namespace LightBulb.Services
{
    public class WindowService : WinApiServiceBase, IDisposable
    {
        private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild, uint dwEventThread,
            uint dwmsEventTime);

        [DllImport("user32.dll", EntryPoint = "GetForegroundWindow", SetLastError = true)]
        private static extern IntPtr GetForegroundWindowInternal();

        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow", SetLastError = true)]
        private static extern IntPtr GetDesktopWindowInternal();

        [DllImport("user32.dll", EntryPoint = "GetShellWindow", SetLastError = true)]
        private static extern IntPtr GetShellWindowInternal();

        [DllImport("user32.dll", EntryPoint = "GetWindowRect", SetLastError = true)]
        private static extern int GetWindowRectInternal(IntPtr hWindow, out Rect rect);

        [DllImport("user32.dll", EntryPoint = "SetWinEventHook", SetLastError = true)]
        private static extern IntPtr SetWinEventHookInternal(uint eventMin, uint eventMax,
            IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc,
            uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll", EntryPoint = "UnhookWinEvent", SetLastError = true)]
        private static extern bool UnhookWinEventInternal(IntPtr hWinEventHook);

        private const uint EventSystemForeground = 3;
        private const uint WineventOutofcontext = 0;

        private readonly WinEventDelegate _foregroundWindowChangedEventHandler;
        private readonly IntPtr _foregroundWindowHook;

        public Settings Settings => Settings.Default;
        public bool IsFullScreen { get; private set; }

        public event EventHandler FullScreenStateChanged;

        public WindowService()
        {
            _foregroundWindowChangedEventHandler =
                (hook, type, hwnd, idObject, child, thread, time) => ForegroundWindowChanged();
            _foregroundWindowHook = SetWinEventHookInternal(EventSystemForeground, EventSystemForeground, IntPtr.Zero,
                _foregroundWindowChangedEventHandler, 0, 0, WineventOutofcontext);
        }

        private void ForegroundWindowChanged()
        {
            // Check if foreground window is fullscreen
            bool fs = IsWindowFullScreen(GetForegroundWindow());
            if (fs != IsFullScreen)
            {
                IsFullScreen = fs;
                FullScreenStateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public IntPtr GetForegroundWindow()
        {
            var result = GetForegroundWindowInternal();
            CheckThrowWin32Error();
            return result;
        }

        public IntPtr GetDesktopWindow()
        {
            var result = GetDesktopWindowInternal();
            CheckThrowWin32Error();
            return result;
        }

        public IntPtr GetShellWindow()
        {
            var result = GetShellWindowInternal();
            CheckThrowWin32Error();
            return result;
        }

        public Rect GetWindowRect(IntPtr hWindow)
        {
            Rect result;
            GetWindowRectInternal(hWindow, out result);
            CheckThrowWin32Error();
            return result;
        }

        public bool IsWindowFullScreen(IntPtr hWindow)
        {
            // Get foreground window
            var foreground = GetForegroundWindow();
            if (foreground == IntPtr.Zero) return false;

            // Get desktop and shell
            var desktop = GetDesktopWindow();
            var shell = GetShellWindow();

            // If foreground is desktop or shell - return
            if (foreground == desktop || foreground == shell) return false;

            // Get the window rect
            var windowRect = GetWindowRect(foreground);

            // If window rect has retarded values, it's most likely a fullscreen game
            if (windowRect.Left <= 0 && windowRect.Top <= 0 && windowRect.Right <= 0 && windowRect.Bottom <= 0)
                return true;

            // Get the screen rect and compare
            var screenRect = Screen.FromHandle(foreground).Bounds;
            return windowRect.Left <= 0 && windowRect.Top <= 0 &&
                   windowRect.Height >= screenRect.Height && windowRect.Width >= screenRect.Width;
        }

        public void Dispose()
        {
            UnhookWinEventInternal(_foregroundWindowHook);
        }
    }
}