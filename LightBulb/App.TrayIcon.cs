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
        var localizationManager = _services.GetRequiredService<LocalizationManager>();
        var mainViewModel = _services.GetRequiredService<ViewModelManager>().GetMainViewModel();

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
                            mainViewModel.Dashboard.CurrentConfiguration.Temperature.ToString("F0")
                            + " / "
                            + mainViewModel.Dashboard.CurrentConfiguration.Brightness.ToString(
                                "P0"
                            );

                        var tooltip =
                            "LightBulb"
                            + Environment.NewLine
                            + (mainViewModel.Dashboard.IsActive ? status : "Disabled");

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
            localizationManager.WatchProperty(
                o => o.Language,
                _ =>
                {
                    try
                    {
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            _trayShowHideItem?.Header = localizationManager.TrayShowHideMenuItem;
                            _traySettingsItem?.Header = localizationManager.TraySettingsMenuItem;
                            _trayToggleItem?.Header = localizationManager.TrayToggleMenuItem;
                            _trayDisableItem?.Header = localizationManager.TrayDisableMenuItem;
                            _trayDisableUntilSunriseItem?.Header =
                                localizationManager.TrayDisableUntilSunriseMenuItem;
                            _trayDisableFor1DayItem?.Header =
                                localizationManager.TrayDisableFor1DayMenuItem;
                            _trayDisableFor12HoursItem?.Header =
                                localizationManager.TrayDisableFor12HoursMenuItem;
                            _trayDisableFor6HoursItem?.Header =
                                localizationManager.TrayDisableFor6HoursMenuItem;
                            _trayDisableFor3HoursItem?.Header =
                                localizationManager.TrayDisableFor3HoursMenuItem;
                            _trayDisableFor1HourItem?.Header =
                                localizationManager.TrayDisableFor1HourMenuItem;
                            _trayDisableFor30MinutesItem?.Header =
                                localizationManager.TrayDisableFor30MinutesMenuItem;
                            _trayDisableFor15MinutesItem?.Header =
                                localizationManager.TrayDisableFor15MinutesMenuItem;
                            _trayDisableFor5MinutesItem?.Header =
                                localizationManager.TrayDisableFor5MinutesMenuItem;
                            _trayDisableFor1MinuteItem?.Header =
                                localizationManager.TrayDisableFor1MinuteMenuItem;
                            _trayExitItem?.Header = localizationManager.TrayExitMenuItem;
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
        var localizationManager = _services.GetRequiredService<LocalizationManager>();
        var mainViewModel = _services.GetRequiredService<ViewModelManager>().GetMainViewModel();

        _trayShowHideItem = new NativeMenuItem(localizationManager.TrayShowHideMenuItem);
        _trayShowHideItem.Click += (_, _) => ToggleMainWindow();

        _traySettingsItem = new NativeMenuItem(localizationManager.TraySettingsMenuItem);
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

            mainViewModel.ShowSettingsCommand.ExecuteIfCan(null);
        };

        _trayToggleItem = new NativeMenuItem(localizationManager.TrayToggleMenuItem);
        _trayToggleItem.Click += (_, _) =>
            mainViewModel.Dashboard.IsEnabled = !mainViewModel.Dashboard.IsEnabled;

        _trayDisableUntilSunriseItem = new NativeMenuItem(
            localizationManager.TrayDisableUntilSunriseMenuItem
        );
        _trayDisableUntilSunriseItem.Click += (_, _) =>
            mainViewModel.Dashboard.DisableUntilSunriseCommand.ExecuteIfCan(null);

        _trayDisableFor1DayItem = new NativeMenuItem(
            localizationManager.TrayDisableFor1DayMenuItem
        );
        _trayDisableFor1DayItem.Click += (_, _) =>
            mainViewModel.Dashboard.DisableTemporarilyCommand.ExecuteIfCan(TimeSpan.FromDays(1));

        _trayDisableFor12HoursItem = new NativeMenuItem(
            localizationManager.TrayDisableFor12HoursMenuItem
        );
        _trayDisableFor12HoursItem.Click += (_, _) =>
            mainViewModel.Dashboard.DisableTemporarilyCommand.ExecuteIfCan(TimeSpan.FromHours(12));

        _trayDisableFor6HoursItem = new NativeMenuItem(
            localizationManager.TrayDisableFor6HoursMenuItem
        );
        _trayDisableFor6HoursItem.Click += (_, _) =>
            mainViewModel.Dashboard.DisableTemporarilyCommand.ExecuteIfCan(TimeSpan.FromHours(6));

        _trayDisableFor3HoursItem = new NativeMenuItem(
            localizationManager.TrayDisableFor3HoursMenuItem
        );
        _trayDisableFor3HoursItem.Click += (_, _) =>
            mainViewModel.Dashboard.DisableTemporarilyCommand.ExecuteIfCan(TimeSpan.FromHours(3));

        _trayDisableFor1HourItem = new NativeMenuItem(
            localizationManager.TrayDisableFor1HourMenuItem
        );
        _trayDisableFor1HourItem.Click += (_, _) =>
            mainViewModel.Dashboard.DisableTemporarilyCommand.ExecuteIfCan(TimeSpan.FromHours(1));

        _trayDisableFor30MinutesItem = new NativeMenuItem(
            localizationManager.TrayDisableFor30MinutesMenuItem
        );
        _trayDisableFor30MinutesItem.Click += (_, _) =>
            mainViewModel.Dashboard.DisableTemporarilyCommand.ExecuteIfCan(
                TimeSpan.FromMinutes(30)
            );

        _trayDisableFor15MinutesItem = new NativeMenuItem(
            localizationManager.TrayDisableFor15MinutesMenuItem
        );
        _trayDisableFor15MinutesItem.Click += (_, _) =>
            mainViewModel.Dashboard.DisableTemporarilyCommand.ExecuteIfCan(
                TimeSpan.FromMinutes(15)
            );

        _trayDisableFor5MinutesItem = new NativeMenuItem(
            localizationManager.TrayDisableFor5MinutesMenuItem
        );
        _trayDisableFor5MinutesItem.Click += (_, _) =>
            mainViewModel.Dashboard.DisableTemporarilyCommand.ExecuteIfCan(TimeSpan.FromMinutes(5));

        _trayDisableFor1MinuteItem = new NativeMenuItem(
            localizationManager.TrayDisableFor1MinuteMenuItem
        );
        _trayDisableFor1MinuteItem.Click += (_, _) =>
            mainViewModel.Dashboard.DisableTemporarilyCommand.ExecuteIfCan(TimeSpan.FromMinutes(1));

        _trayDisableItem = new NativeMenuItem(localizationManager.TrayDisableMenuItem)
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

        _trayExitItem = new NativeMenuItem(localizationManager.TrayExitMenuItem);
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
