using System;
using Avalonia.Controls;
using Avalonia.Platform;
using LightBulb.Localization;
using LightBulb.Utils.Extensions;
using LightBulb.ViewModels.Components;
using LightBulb.Views.Controls;

namespace LightBulb;

public partial class App
{
    private BindableTrayIcon? _trayIcon;

    private void InitializeTrayIcon()
    {
        BindableNativeMenuItem CreateLocalizedItem(
            string propertyName,
            Func<LocalizationManager, string> getValue
        )
        {
            var item = new BindableNativeMenuItem();
            item.Bind(
                NativeMenuItem.HeaderProperty,
                _localizationManager.ObserveProperty(propertyName, getValue)
            );
            return item;
        }

        var trayShowHideItem = CreateLocalizedItem(
            nameof(LocalizationManager.TrayShowHideMenuItem),
            static m => m.TrayShowHideMenuItem
        );
        trayShowHideItem.Click += (_, _) => ToggleMainWindow();

        var traySettingsItem = CreateLocalizedItem(
            nameof(LocalizationManager.TraySettingsMenuItem),
            static m => m.TraySettingsMenuItem
        );
        traySettingsItem.Click += async (_, _) =>
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
        };

        var trayToggleItem = CreateLocalizedItem(
            nameof(LocalizationManager.TrayToggleMenuItem),
            static m => m.TrayToggleMenuItem
        );
        trayToggleItem.Click += (_, _) =>
            _mainViewModel.Dashboard.IsEnabled = !_mainViewModel.Dashboard.IsEnabled;

        var trayDisableUntilSunriseItem = CreateLocalizedItem(
            nameof(LocalizationManager.TrayDisableUntilSunriseMenuItem),
            static m => m.TrayDisableUntilSunriseMenuItem
        );
        trayDisableUntilSunriseItem.Click += (_, _) =>
            _mainViewModel.Dashboard.DisableUntilSunriseCommand.Execute(null);

        var trayDisableFor1DayItem = CreateLocalizedItem(
            nameof(LocalizationManager.TrayDisableFor1DayMenuItem),
            static m => m.TrayDisableFor1DayMenuItem
        );
        trayDisableFor1DayItem.Click += (_, _) =>
            _mainViewModel.Dashboard.DisableTemporarilyCommand.Execute(TimeSpan.FromDays(1));

        var trayDisableFor12HoursItem = CreateLocalizedItem(
            nameof(LocalizationManager.TrayDisableFor12HoursMenuItem),
            static m => m.TrayDisableFor12HoursMenuItem
        );
        trayDisableFor12HoursItem.Click += (_, _) =>
            _mainViewModel.Dashboard.DisableTemporarilyCommand.Execute(TimeSpan.FromHours(12));

        var trayDisableFor6HoursItem = CreateLocalizedItem(
            nameof(LocalizationManager.TrayDisableFor6HoursMenuItem),
            static m => m.TrayDisableFor6HoursMenuItem
        );
        trayDisableFor6HoursItem.Click += (_, _) =>
            _mainViewModel.Dashboard.DisableTemporarilyCommand.Execute(TimeSpan.FromHours(6));

        var trayDisableFor3HoursItem = CreateLocalizedItem(
            nameof(LocalizationManager.TrayDisableFor3HoursMenuItem),
            static m => m.TrayDisableFor3HoursMenuItem
        );
        trayDisableFor3HoursItem.Click += (_, _) =>
            _mainViewModel.Dashboard.DisableTemporarilyCommand.Execute(TimeSpan.FromHours(3));

        var trayDisableFor1HourItem = CreateLocalizedItem(
            nameof(LocalizationManager.TrayDisableFor1HourMenuItem),
            static m => m.TrayDisableFor1HourMenuItem
        );
        trayDisableFor1HourItem.Click += (_, _) =>
            _mainViewModel.Dashboard.DisableTemporarilyCommand.Execute(TimeSpan.FromHours(1));

        var trayDisableFor30MinutesItem = CreateLocalizedItem(
            nameof(LocalizationManager.TrayDisableFor30MinutesMenuItem),
            static m => m.TrayDisableFor30MinutesMenuItem
        );
        trayDisableFor30MinutesItem.Click += (_, _) =>
            _mainViewModel.Dashboard.DisableTemporarilyCommand.Execute(TimeSpan.FromMinutes(30));

        var trayDisableFor15MinutesItem = CreateLocalizedItem(
            nameof(LocalizationManager.TrayDisableFor15MinutesMenuItem),
            static m => m.TrayDisableFor15MinutesMenuItem
        );
        trayDisableFor15MinutesItem.Click += (_, _) =>
            _mainViewModel.Dashboard.DisableTemporarilyCommand.Execute(TimeSpan.FromMinutes(15));

        var trayDisableFor5MinutesItem = CreateLocalizedItem(
            nameof(LocalizationManager.TrayDisableFor5MinutesMenuItem),
            static m => m.TrayDisableFor5MinutesMenuItem
        );
        trayDisableFor5MinutesItem.Click += (_, _) =>
            _mainViewModel.Dashboard.DisableTemporarilyCommand.Execute(TimeSpan.FromMinutes(5));

        var trayDisableFor1MinuteItem = CreateLocalizedItem(
            nameof(LocalizationManager.TrayDisableFor1MinuteMenuItem),
            static m => m.TrayDisableFor1MinuteMenuItem
        );
        trayDisableFor1MinuteItem.Click += (_, _) =>
            _mainViewModel.Dashboard.DisableTemporarilyCommand.Execute(TimeSpan.FromMinutes(1));

        var trayDisableItem = CreateLocalizedItem(
            nameof(LocalizationManager.TrayDisableMenuItem),
            static m => m.TrayDisableMenuItem
        );

        trayDisableItem.Menu = new NativeMenu
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
        };

        var trayExitItem = CreateLocalizedItem(
            nameof(LocalizationManager.TrayExitMenuItem),
            static m => m.TrayExitMenuItem
        );
        trayExitItem.Click += (_, _) =>
        {
            if (ApplicationLifetime?.TryShutdown() != true)
                Environment.Exit(0);
        };

        _trayIcon = new BindableTrayIcon
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

        _trayIcon.Bind(
            TrayIcon.ToolTipTextProperty,
            _mainViewModel.Dashboard.ObserveProperty(
                nameof(DashboardViewModel.TrayTooltip),
                static d => d.TrayTooltip
            )
        );

        _trayIcon.Clicked += (_, _) => ToggleMainWindow();
        _trayIcon.AttachToApplication(this);
    }
}
