using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Threading;
using LightBulb.Utils.Extensions;

namespace LightBulb;

public partial class App
{
    // Tray menu items — kept as fields so headers can be updated without index-based lookups
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
        // Tray icon does not support binding so we use this hack to synchronize its menu and tooltip
        _eventRoot.Add(
            _localizationManager.WatchProperty(
                o => o.Language,
                () =>
                {
                    try
                    {
                        Dispatcher.UIThread.Invoke(() =>
                        {
                            if (_trayShowHideItem is not null)
                                _trayShowHideItem.Header =
                                    _localizationManager.TrayShowHideMenuItem;
                            if (_traySettingsItem is not null)
                                _traySettingsItem.Header =
                                    _localizationManager.TraySettingsMenuItem;
                            if (_trayToggleItem is not null)
                                _trayToggleItem.Header = _localizationManager.TrayToggleMenuItem;
                            if (_trayDisableItem is not null)
                                _trayDisableItem.Header = _localizationManager.TrayDisableMenuItem;
                            if (_trayDisableUntilSunriseItem is not null)
                                _trayDisableUntilSunriseItem.Header =
                                    _localizationManager.TrayDisableUntilSunriseMenuItem;
                            if (_trayDisableFor1DayItem is not null)
                                _trayDisableFor1DayItem.Header =
                                    _localizationManager.TrayDisableFor1DayMenuItem;
                            if (_trayDisableFor12HoursItem is not null)
                                _trayDisableFor12HoursItem.Header =
                                    _localizationManager.TrayDisableFor12HoursMenuItem;
                            if (_trayDisableFor6HoursItem is not null)
                                _trayDisableFor6HoursItem.Header =
                                    _localizationManager.TrayDisableFor6HoursMenuItem;
                            if (_trayDisableFor3HoursItem is not null)
                                _trayDisableFor3HoursItem.Header =
                                    _localizationManager.TrayDisableFor3HoursMenuItem;
                            if (_trayDisableFor1HourItem is not null)
                                _trayDisableFor1HourItem.Header =
                                    _localizationManager.TrayDisableFor1HourMenuItem;
                            if (_trayDisableFor30MinutesItem is not null)
                                _trayDisableFor30MinutesItem.Header =
                                    _localizationManager.TrayDisableFor30MinutesMenuItem;
                            if (_trayDisableFor15MinutesItem is not null)
                                _trayDisableFor15MinutesItem.Header =
                                    _localizationManager.TrayDisableFor15MinutesMenuItem;
                            if (_trayDisableFor5MinutesItem is not null)
                                _trayDisableFor5MinutesItem.Header =
                                    _localizationManager.TrayDisableFor5MinutesMenuItem;
                            if (_trayDisableFor1MinuteItem is not null)
                                _trayDisableFor1MinuteItem.Header =
                                    _localizationManager.TrayDisableFor1MinuteMenuItem;
                            if (_trayExitItem is not null)
                                _trayExitItem.Header = _localizationManager.TrayExitMenuItem;
                        });
                    }
                    // Ignore exceptions when the application is shutting down
                    catch (OperationCanceledException) { }
                }
            )
        );

        _eventRoot.Add(
            _mainViewModel.Dashboard.WatchProperties(
                [o => o.IsActive, o => o.CurrentConfiguration],
                () =>
                {
                    var status =
                        _mainViewModel.Dashboard.CurrentConfiguration.Temperature.ToString("F0")
                        + " / "
                        + _mainViewModel.Dashboard.CurrentConfiguration.Brightness.ToString("P0");

                    var tooltip =
                        "LightBulb"
                        + Environment.NewLine
                        + (_mainViewModel.Dashboard.IsActive ? status : "Disabled");

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
    }

    private void InitializeTrayIcon()
    {
        _trayShowHideItem = new NativeMenuItem(_localizationManager.TrayShowHideMenuItem);
        _trayShowHideItem.Click += TrayIcon_OnClicked;

        _traySettingsItem = new NativeMenuItem(_localizationManager.TraySettingsMenuItem);
        _traySettingsItem.Click += ShowSettingsMenuItem_OnClick;

        _trayToggleItem = new NativeMenuItem(_localizationManager.TrayToggleMenuItem);
        _trayToggleItem.Click += ToggleMenuItem_OnClick;

        _trayDisableUntilSunriseItem = new NativeMenuItem(
            _localizationManager.TrayDisableUntilSunriseMenuItem
        );
        _trayDisableUntilSunriseItem.Click += DisableUntilSunriseMenuItem_OnClick;

        _trayDisableFor1DayItem = new NativeMenuItem(
            _localizationManager.TrayDisableFor1DayMenuItem
        );
        _trayDisableFor1DayItem.Click += DisableTemporarily1DayMenuItem_OnClick;

        _trayDisableFor12HoursItem = new NativeMenuItem(
            _localizationManager.TrayDisableFor12HoursMenuItem
        );
        _trayDisableFor12HoursItem.Click += DisableTemporarily12HoursMenuItem_OnClick;

        _trayDisableFor6HoursItem = new NativeMenuItem(
            _localizationManager.TrayDisableFor6HoursMenuItem
        );
        _trayDisableFor6HoursItem.Click += DisableTemporarily6HoursMenuItem_OnClick;

        _trayDisableFor3HoursItem = new NativeMenuItem(
            _localizationManager.TrayDisableFor3HoursMenuItem
        );
        _trayDisableFor3HoursItem.Click += DisableTemporarily3HoursMenuItem_OnClick;

        _trayDisableFor1HourItem = new NativeMenuItem(
            _localizationManager.TrayDisableFor1HourMenuItem
        );
        _trayDisableFor1HourItem.Click += DisableTemporarily1HourMenuItem_OnClick;

        _trayDisableFor30MinutesItem = new NativeMenuItem(
            _localizationManager.TrayDisableFor30MinutesMenuItem
        );
        _trayDisableFor30MinutesItem.Click += DisableTemporarily30MinutesMenuItem_OnClick;

        _trayDisableFor15MinutesItem = new NativeMenuItem(
            _localizationManager.TrayDisableFor15MinutesMenuItem
        );
        _trayDisableFor15MinutesItem.Click += DisableTemporarily15MinutesMenuItem_OnClick;

        _trayDisableFor5MinutesItem = new NativeMenuItem(
            _localizationManager.TrayDisableFor5MinutesMenuItem
        );
        _trayDisableFor5MinutesItem.Click += DisableTemporarily5MinutesMenuItem_OnClick;

        _trayDisableFor1MinuteItem = new NativeMenuItem(
            _localizationManager.TrayDisableFor1MinuteMenuItem
        );
        _trayDisableFor1MinuteItem.Click += DisableTemporarily1MinuteMenuItem_OnClick;

        var disableSubMenu = new NativeMenu();
        disableSubMenu.Items.Add(_trayDisableUntilSunriseItem);
        disableSubMenu.Items.Add(_trayDisableFor1DayItem);
        disableSubMenu.Items.Add(_trayDisableFor12HoursItem);
        disableSubMenu.Items.Add(_trayDisableFor6HoursItem);
        disableSubMenu.Items.Add(_trayDisableFor3HoursItem);
        disableSubMenu.Items.Add(_trayDisableFor1HourItem);
        disableSubMenu.Items.Add(_trayDisableFor30MinutesItem);
        disableSubMenu.Items.Add(_trayDisableFor15MinutesItem);
        disableSubMenu.Items.Add(_trayDisableFor5MinutesItem);
        disableSubMenu.Items.Add(_trayDisableFor1MinuteItem);

        _trayDisableItem = new NativeMenuItem(_localizationManager.TrayDisableMenuItem)
        {
            Menu = disableSubMenu,
        };

        _trayExitItem = new NativeMenuItem(_localizationManager.TrayExitMenuItem);
        _trayExitItem.Click += ExitMenuItem_OnClick;

        var menu = new NativeMenu();
        menu.Items.Add(_trayShowHideItem);
        menu.Items.Add(_traySettingsItem);
        menu.Items.Add(new NativeMenuItemSeparator());
        menu.Items.Add(_trayToggleItem);
        menu.Items.Add(_trayDisableItem);
        menu.Items.Add(new NativeMenuItemSeparator());
        menu.Items.Add(_trayExitItem);

        _trayIcon = new TrayIcon
        {
            Icon = new WindowIcon(AssetLoader.Open(new Uri("avares://LightBulb/favicon.ico"))),
            ToolTipText = "LightBulb",
            Menu = menu,
        };
        _trayIcon.Clicked += TrayIcon_OnClicked;

        TrayIcon.SetIcons(this, new TrayIcons { _trayIcon });
    }

    private void TrayIcon_OnClicked(object? sender, EventArgs args) => ToggleMainWindow();

    private async void ShowSettingsMenuItem_OnClick(object? sender, EventArgs args)
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

        _mainViewModel.ShowSettingsCommand.Execute(null);
    }

    private void ToggleMenuItem_OnClick(object? sender, EventArgs args) =>
        _mainViewModel.Dashboard.IsEnabled = !_mainViewModel.Dashboard.IsEnabled;

    private void DisableUntilSunriseMenuItem_OnClick(object? sender, EventArgs args) =>
        _mainViewModel.Dashboard.DisableUntilSunriseCommand.Execute(null);

    private void DisableTemporarily1DayMenuItem_OnClick(object? sender, EventArgs args) =>
        _mainViewModel.Dashboard.DisableTemporarilyCommand.Execute(TimeSpan.FromDays(1));

    private void DisableTemporarily12HoursMenuItem_OnClick(object? sender, EventArgs args) =>
        _mainViewModel.Dashboard.DisableTemporarilyCommand.Execute(TimeSpan.FromHours(12));

    private void DisableTemporarily6HoursMenuItem_OnClick(object? sender, EventArgs args) =>
        _mainViewModel.Dashboard.DisableTemporarilyCommand.Execute(TimeSpan.FromHours(6));

    private void DisableTemporarily3HoursMenuItem_OnClick(object? sender, EventArgs args) =>
        _mainViewModel.Dashboard.DisableTemporarilyCommand.Execute(TimeSpan.FromHours(3));

    private void DisableTemporarily1HourMenuItem_OnClick(object? sender, EventArgs args) =>
        _mainViewModel.Dashboard.DisableTemporarilyCommand.Execute(TimeSpan.FromHours(1));

    private void DisableTemporarily30MinutesMenuItem_OnClick(object? sender, EventArgs args) =>
        _mainViewModel.Dashboard.DisableTemporarilyCommand.Execute(TimeSpan.FromMinutes(30));

    private void DisableTemporarily15MinutesMenuItem_OnClick(object? sender, EventArgs args) =>
        _mainViewModel.Dashboard.DisableTemporarilyCommand.Execute(TimeSpan.FromMinutes(15));

    private void DisableTemporarily5MinutesMenuItem_OnClick(object? sender, EventArgs args) =>
        _mainViewModel.Dashboard.DisableTemporarilyCommand.Execute(TimeSpan.FromMinutes(5));

    private void DisableTemporarily1MinuteMenuItem_OnClick(object? sender, EventArgs args) =>
        _mainViewModel.Dashboard.DisableTemporarilyCommand.Execute(TimeSpan.FromMinutes(1));

    private void ExitMenuItem_OnClick(object? sender, EventArgs args)
    {
        if (ApplicationLifetime?.TryShutdown() != true)
            Environment.Exit(0);
    }
}
