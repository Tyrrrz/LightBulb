using System;
using LightBulb.ViewModels.Components;
using LightBulb.ViewModels.Dialogs;
using Microsoft.Extensions.DependencyInjection;

namespace LightBulb.ViewModels.Framework;

public class ViewModelProvider(IServiceProvider services)
{
    public MainViewModel GetMainViewModel() => services.GetRequiredService<MainViewModel>();

    public DashboardViewModel GetDashboardViewModel() =>
        services.GetRequiredService<DashboardViewModel>();

    public MessageBoxViewModelModel GetMessageBoxViewModel(
        string title,
        string message,
        string? okButtonText,
        string? cancelButtonText
    )
    {
        var viewModel = services.GetRequiredService<MessageBoxViewModelModel>();

        viewModel.Title = title;
        viewModel.Message = message;
        viewModel.IsDefaultButtonVisible = !string.IsNullOrWhiteSpace(okButtonText);
        viewModel.DefaultButtonText = okButtonText;
        viewModel.IsCancelButtonVisible = !string.IsNullOrWhiteSpace(cancelButtonText);
        viewModel.CancelButtonText = cancelButtonText;

        return viewModel;
    }

    public SettingsViewModelModel GetSettingsViewModel() =>
        services.GetRequiredService<SettingsViewModelModel>();
}
