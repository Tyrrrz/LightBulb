﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace LightBulb.WindowsApi.Internal
{
    internal static partial class NativeMethods
    {
        public delegate void WinEventHandler(IntPtr hWinEventHook, uint eventType, IntPtr hWnd, int idObject,
            int idChild, uint idEventThread, uint dwmsEventTime);
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal static partial class NativeMethods
    {
        [DllImport("user32.dll", EntryPoint = "SetWinEventHook", SetLastError = true)]
        public static extern IntPtr SetWinEventHook(
            uint eventMin, uint eventMax,
            IntPtr hmodWinEventProc, WinEventHandler lpfnWinEventProc,
            uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll", EntryPoint = "UnhookWinEvent", SetLastError = true)]
        public static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32.dll", EntryPoint = "GetDC", SetLastError = true)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "ReleaseDC", SetLastError = true)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", EntryPoint = "SetDeviceGammaRamp", SetLastError = true)]
        public static extern bool SetDeviceGammaRamp(IntPtr hDC, ref GammaRamp lpRamp);

        [DllImport("gdi32.dll", EntryPoint = "GetDeviceGammaRamp", SetLastError = true)]
        public static extern bool GetDeviceGammaRamp(IntPtr hDC, out GammaRamp lpRamp);

        [DllImport("user32.dll", EntryPoint = "RegisterHotKey", SetLastError = true)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll", EntryPoint = "UnregisterHotKey", SetLastError = true)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll", EntryPoint = "GetForegroundWindow", SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", EntryPoint = "GetWindowRect", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);

        [DllImport("user32.dll", EntryPoint = "GetClientRect", SetLastError = true)]
        public static extern bool GetClientRect(IntPtr hWnd, out Rect lpRect);

        [DllImport("user32.dll", EntryPoint = "IsWindowVisible", SetLastError = true)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "GetClassName", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
    }
}