using System;
using System.ComponentModel;
using Microsoft.Win32;

namespace LightBulb.PlatformInterop;

/// <summary>
/// Windows implementation of <see cref="IPlatformSettings"/>.
/// Persists settings in the Windows registry.
/// </summary>
public class PlatformSettings(string appName, string autoStartCommand) : IPlatformSettings
{
    private readonly RegistrySwitch<string> _autoStartSwitch = new(
        RegistryHive.CurrentUser,
        @"Software\Microsoft\Windows\CurrentVersion\Run",
        appName,
        autoStartCommand
    );

    private readonly RegistrySwitch<int> _extendedGammaRangeSwitch = new(
        RegistryHive.LocalMachine,
        @"Software\Microsoft\Windows NT\CurrentVersion\ICM",
        "GdiICMGammaRange",
        256
    );

    public bool IsAutoStartEnabled
    {
        get
        {
            try
            {
                return _autoStartSwitch.IsSet;
            }
            catch (Exception ex) when (ex is Win32Exception or UnauthorizedAccessException)
            {
                return false;
            }
        }
        set
        {
            try
            {
                _autoStartSwitch.IsSet = value;
            }
            catch (Exception ex) when (ex is Win32Exception or UnauthorizedAccessException)
            {
                // Ignore permission errors — the setting is best-effort.
            }
        }
    }

    public bool IsExtendedGammaRangeUnlocked
    {
        get
        {
            try
            {
                return _extendedGammaRangeSwitch.IsSet;
            }
            catch (Exception ex) when (ex is Win32Exception or UnauthorizedAccessException)
            {
                return false;
            }
        }
        set
        {
            try
            {
                _extendedGammaRangeSwitch.IsSet = value;
            }
            catch (Exception ex) when (ex is Win32Exception or UnauthorizedAccessException)
            {
                // Ignore permission errors — the setting is best-effort.
            }
        }
    }
}
