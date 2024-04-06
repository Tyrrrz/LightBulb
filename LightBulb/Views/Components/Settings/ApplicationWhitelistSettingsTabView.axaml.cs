using System;
using System.Linq;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Interactivity;
using LightBulb.Framework;
using LightBulb.Models;
using LightBulb.Utils;
using LightBulb.Utils.Extensions;
using LightBulb.ViewModels.Components.Settings;

namespace LightBulb.Views.Components.Settings;

public partial class ApplicationWhitelistSettingsTabView
    : UserControl<ApplicationWhitelistSettingsTabViewModel>,
        IDisposable
{
    private readonly DisposableCollector _eventRoot = new();

    public ApplicationWhitelistSettingsTabView() => InitializeComponent();

    private void UserControl_OnLoaded(object? sender, RoutedEventArgs args)
    {
        DataContext.RefreshApplicationsCommand.Execute(null);

        _eventRoot.Add(
            // This hack is required to avoid having to use an ObservableCollection<T> on the view model
            DataContext.WatchProperty(
                o => o.WhitelistedApplications,
                () =>
                    WhitelistedApplicationsListBox.SelectedItems = new AvaloniaList<object>(
                        DataContext.WhitelistedApplications ?? []
                    )
            )
        );
    }

    private void UserControl_OnUnloaded(object? sender, RoutedEventArgs args) =>
        _eventRoot.Dispose();

    // This hack is required to avoid having to use an ObservableCollection<T> on the view model
    private void WhitelistedApplicationsListBox_OnSelectionChanged(
        object? sender,
        SelectionChangedEventArgs args
    ) =>
        DataContext.WhitelistedApplications = WhitelistedApplicationsListBox
            .SelectedItems?.Cast<ExternalApplication>()
            .ToArray();

    public void Dispose() => _eventRoot.Dispose();
}
