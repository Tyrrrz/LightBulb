using System;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Threading;
using LightBulb.Framework;
using LightBulb.Localization;
using LightBulb.Utils.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace LightBulb;

// Tray icon and its menu do not support XAML binding, so we construct it in code and update it ourselves
public partial class App
{
    private NativeMenuItem? _trayShowHideItem;
    private NativeMenuItem? _traySettingsItem;
    private NativeMenuItem? _trayToggleItem;
    private NativeMenuItem? _trayDisableItem;
    private NativeMenuItem? _trayDisableUntilSunriseItem;
    private NativeMenuItem? _trayDisableFor1DayItem;
    private NativeMenuItem? _trayDisableFor12HoursItem;
    private NativeMenuItem? _trayDisableFor6HoursItem;
    private NativeMenuItem? _trayDisableFor3HoursItem;
    private NativeMenuItem? _trayDisableFor1HourItem;
    private NativeMenuItem? _trayDisableFor30MinutesItem;
    private NativeMenuItem? _trayDisableFor15MinutesItem;
    private NativeMenuItem? _trayDisableFor5MinutesItem;
    private NativeMenuItem? _trayDisableFor1MinuteItem;
    private NativeMenuItem? _trayExitItem;
    private TrayIcon? _trayIcon;

    private void RegisterTrayIconEvents()
    {
        // Update the tooltip when the dashboard state changes
        _eventRoot.Add(
            _services
                .GetRequiredService<ViewModelManager>()
                .GetMainViewModel()
                .Dashboard.WatchProperties(
                    [o => o.IsActive, o => o.CurrentConfiguration],
                    () =>
                    {
                        var status =
                            _services
                                .GetRequiredService<ViewModelManager>()
                                .GetMainViewModel()
                                .Dashboard.CurrentConfiguration.Temperature.ToString("F0")
                            + " / "
                            + _services
                                .GetRequiredService<ViewModelManager>()
                                .GetMainViewModel()
                                .Dashboard.CurrentConfiguration.Brightness.ToString("P0");

                        var tooltip =
                            "LightBulb"
                            + Environment.NewLine
                            + (
                                _services
                                    .GetRequiredService<ViewModelManager>()
                                    .GetMainViewModel()
                                    .Dashboard.IsActive
                                    ? status
                                    : "Disabled"
                            );

                        try
                        {
                            Dispatcher.UIThread.Invoke(() =>
                            {
                                if (_trayIcon is { } trayIcon)
                                    trayIcon.ToolTipText = tooltip;
                            });
                        }
                        // Ignore exceptions when the application is shutting down
                        catch (OperationCanceledException) { }
                    }
                )
        );

        // Update menu item headers when the language changes
        _eventRoot.Add(
            _services
                .GetRequiredService<LocalizationManager>()
                .WatchProperty(
                    o => o.Language,
                    () =>
                    {
                        try
                        {
                            Dispatcher.UIThread.Invoke(() =>
                            {
                                _trayShowHideItem?.Header = _services
                                    .GetRequiredService<LocalizationManager>()
                                    .TrayShowHideMenuItem;
                                _traySettingsItem?.Header = _services
                                    .GetRequiredService<LocalizationManager>()
                                    .TraySettingsMenuItem;
                                _trayToggleItem?.Header = _services
                                    .GetRequiredService<LocalizationManager>()
                                    .TrayToggleMenuItem;
                                _trayDisableItem?.Header = _services
                                    .GetRequiredService<LocalizationManager>()
                                    .TrayDisableMenuItem;
                                _trayDisableUntilSunriseItem?.Header = _services
                                    .GetRequiredService<LocalizationManager>()
                                    .TrayDisableUntilSunriseMenuItem;
                                _trayDisableFor1DayItem?.Header = _services
                                    .GetRequiredService<LocalizationManager>()
                                    .TrayDisableFor1DayMenuItem;
                                _trayDisableFor12HoursItem?.Header = _services
                                    .GetRequiredService<LocalizationManager>()
                                    .TrayDisableFor12HoursMenuItem;
                                _trayDisableFor6HoursItem?.Header = _services
                                    .GetRequiredService<LocalizationManager>()
                                    .TrayDisableFor6HoursMenuItem;
                                _trayDisableFor3HoursItem?.Header = _services
                                    .GetRequiredService<LocalizationManager>()
                                    .TrayDisableFor3HoursMenuItem;
                                _trayDisableFor1HourItem?.Header = _services
                                    .GetRequiredService<LocalizationManager>()
                                    .TrayDisableFor1HourMenuItem;
                                _trayDisableFor30MinutesItem?.Header = _services
                                    .GetRequiredService<LocalizationManager>()
                                    .TrayDisableFor30MinutesMenuItem;
                                _trayDisableFor15MinutesItem?.Header = _services
                                    .GetRequiredService<LocalizationManager>()
                                    .TrayDisableFor15MinutesMenuItem;
                                _trayDisableFor5MinutesItem?.Header = _services
                                    .GetRequiredService<LocalizationManager>()
                                    .TrayDisableFor5MinutesMenuItem;
                                _trayDisableFor1MinuteItem?.Header = _services
                                    .GetRequiredService<LocalizationManager>()
                                    .TrayDisableFor1MinuteMenuItem;
                                _trayExitItem?.Header = _services
                                    .GetRequiredService<LocalizationManager>()
                                    .TrayExitMenuItem;
                            });
                        }
                        // Ignore exceptions when the application is shutting down
                        catch (OperationCanceledException) { }
                    }
                )
        );
    }

    private void InitializeTrayIcon()
    {
        _trayShowHideItem = new NativeMenuItem(
            _services.GetRequiredService<LocalizationManager>().TrayShowHideMenuItem
        );
        _trayShowHideItem.Click += (_, _) => ToggleMainWindow();

        _traySettingsItem = new NativeMenuItem(
            _services.GetRequiredService<LocalizationManager>().TraySettingsMenuItem
        );
        _traySettingsItem.Click += async (_, _) =>
        {
            var window = ShowMainWindow();
            if (window is null)
                return;

            // Wait until the window is loaded to avoid potential issues
            // with showing a dialog too early in the lifecycle.
            try
            {
                await window.WaitUntilLoadedAsync();
            }
            catch (OperationCanceledException)
            {
                return;
            }

            _services
                .GetRequiredService<ViewModelManager>()
                .GetMainViewModel()
                .ShowSettingsCommand.ExecuteIfCan(null);
        };

        _trayToggleItem = new NativeMenuItem(
            _services.GetRequiredService<LocalizationManager>().TrayToggleMenuItem
        );
        _trayToggleItem.Click += (_, _) =>
            _services
                .GetRequiredService<ViewModelManager>()
                .GetMainViewModel()
                .Dashboard.IsEnabled = !_services
                .GetRequiredService<ViewModelManager>()
                .GetMainViewModel()
                .Dashboard.IsEnabled;

        _trayDisableUntilSunriseItem = new NativeMenuItem(
            _services.GetRequiredService<LocalizationManager>().TrayDisableUntilSunriseMenuItem
        );
        _trayDisableUntilSunriseItem.Click += (_, _) =>
            _services
                .GetRequiredService<ViewModelManager>()
                .GetMainViewModel()
                .Dashboard.DisableUntilSunriseCommand.ExecuteIfCan(null);

        _trayDisableFor1DayItem = new NativeMenuItem(
            _services.GetRequiredService<LocalizationManager>().TrayDisableFor1DayMenuItem
        );
        _trayDisableFor1DayItem.Click += (_, _) =>
            _services
                .GetRequiredService<ViewModelManager>()
                .GetMainViewModel()
                .Dashboard.DisableTemporarilyCommand.ExecuteIfCan(TimeSpan.FromDays(1));

        _trayDisableFor12HoursItem = new NativeMenuItem(
            _services.GetRequiredService<LocalizationManager>().TrayDisableFor12HoursMenuItem
        );
        _trayDisableFor12HoursItem.Click += (_, _) =>
            _services
                .GetRequiredService<ViewModelManager>()
                .GetMainViewModel()
                .Dashboard.DisableTemporarilyCommand.ExecuteIfCan(TimeSpan.FromHours(12));

        _trayDisableFor6HoursItem = new NativeMenuItem(
            _services.GetRequiredService<LocalizationManager>().TrayDisableFor6HoursMenuItem
        );
        _trayDisableFor6HoursItem.Click += (_, _) =>
            _services
                .GetRequiredService<ViewModelManager>()
                .GetMainViewModel()
                .Dashboard.DisableTemporarilyCommand.ExecuteIfCan(TimeSpan.FromHours(6));

        _trayDisableFor3HoursItem = new NativeMenuItem(
            _services.GetRequiredService<LocalizationManager>().TrayDisableFor3HoursMenuItem
        );
        _trayDisableFor3HoursItem.Click += (_, _) =>
            _services
                .GetRequiredService<ViewModelManager>()
                .GetMainViewModel()
                .Dashboard.DisableTemporarilyCommand.ExecuteIfCan(TimeSpan.FromHours(3));

        _trayDisableFor1HourItem = new NativeMenuItem(
            _services.GetRequiredService<LocalizationManager>().TrayDisableFor1HourMenuItem
        );
        _trayDisableFor1HourItem.Click += (_, _) =>
            _services
                .GetRequiredService<ViewModelManager>()
                .GetMainViewModel()
                .Dashboard.DisableTemporarilyCommand.ExecuteIfCan(TimeSpan.FromHours(1));

        _trayDisableFor30MinutesItem = new NativeMenuItem(
            _services.GetRequiredService<LocalizationManager>().TrayDisableFor30MinutesMenuItem
        );
        _trayDisableFor30MinutesItem.Click += (_, _) =>
            _services
                .GetRequiredService<ViewModelManager>()
                .GetMainViewModel()
                .Dashboard.DisableTemporarilyCommand.ExecuteIfCan(TimeSpan.FromMinutes(30));

        _trayDisableFor15MinutesItem = new NativeMenuItem(
            _services.GetRequiredService<LocalizationManager>().TrayDisableFor15MinutesMenuItem
        );
        _trayDisableFor15MinutesItem.Click += (_, _) =>
            _services
                .GetRequiredService<ViewModelManager>()
                .GetMainViewModel()
                .Dashboard.DisableTemporarilyCommand.ExecuteIfCan(TimeSpan.FromMinutes(15));

        _trayDisableFor5MinutesItem = new NativeMenuItem(
            _services.GetRequiredService<LocalizationManager>().TrayDisableFor5MinutesMenuItem
        );
        _trayDisableFor5MinutesItem.Click += (_, _) =>
            _services
                .GetRequiredService<ViewModelManager>()
                .GetMainViewModel()
                .Dashboard.DisableTemporarilyCommand.ExecuteIfCan(TimeSpan.FromMinutes(5));

        _trayDisableFor1MinuteItem = new NativeMenuItem(
            _services.GetRequiredService<LocalizationManager>().TrayDisableFor1MinuteMenuItem
        );
        _trayDisableFor1MinuteItem.Click += (_, _) =>
            _services
                .GetRequiredService<ViewModelManager>()
                .GetMainViewModel()
                .Dashboard.DisableTemporarilyCommand.ExecuteIfCan(TimeSpan.FromMinutes(1));

        _trayDisableItem = new NativeMenuItem(
            _services.GetRequiredService<LocalizationManager>().TrayDisableMenuItem
        )
        {
            Menu = new NativeMenu
            {
                Items =
                {
                    _trayDisableUntilSunriseItem,
                    _trayDisableFor1DayItem,
                    _trayDisableFor12HoursItem,
                    _trayDisableFor6HoursItem,
                    _trayDisableFor3HoursItem,
                    _trayDisableFor1HourItem,
                    _trayDisableFor30MinutesItem,
                    _trayDisableFor15MinutesItem,
                    _trayDisableFor5MinutesItem,
                    _trayDisableFor1MinuteItem,
                },
            },
        };

        _trayExitItem = new NativeMenuItem(
            _services.GetRequiredService<LocalizationManager>().TrayExitMenuItem
        );
        _trayExitItem.Click += (_, _) =>
        {
            if (ApplicationLifetime?.TryShutdown() != true)
                Environment.Exit(0);
        };

        _trayIcon = new TrayIcon
        {
            Icon = new WindowIcon(AssetLoader.Open(new Uri("avares://LightBulb/favicon.ico"))),
            ToolTipText = "LightBulb",
            Menu = new NativeMenu
            {
                Items =
                {
                    _trayShowHideItem,
                    _traySettingsItem,
                    new NativeMenuItemSeparator(),
                    _trayToggleItem,
                    _trayDisableItem,
                    new NativeMenuItemSeparator(),
                    _trayExitItem,
                },
            },
        };
        _trayIcon.Clicked += (_, _) => ToggleMainWindow();

        TrayIcon.SetIcons(this, [_trayIcon]);
    }
}
