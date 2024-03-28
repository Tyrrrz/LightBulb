using System;
using LightBulb.ViewModels;
using LightBulb.ViewModels.Components;
using LightBulb.ViewModels.Dialogs;
using Microsoft.Extensions.DependencyInjection;

namespace LightBulb.Framework;

public class ViewModelManager(IServiceProvider services)
{
    public MainViewModel CreateMainViewModel() => services.GetRequiredService<MainViewModel>();

    public DashboardViewModel CreateDashboardViewModel() =>
        services.GetRequiredService<DashboardViewModel>();

    public MessageBoxViewModel CreateMessageBoxViewModel(
        string title,
        string message,
        string? okButtonText,
        string? cancelButtonText
    )
    {
        var viewModel = services.GetRequiredService<MessageBoxViewModel>();

        viewModel.Title = title;
        viewModel.Message = message;
        viewModel.DefaultButtonText = okButtonText;
        viewModel.CancelButtonText = cancelButtonText;

        return viewModel;
    }

    public SettingsViewModel CreateSettingsViewModel() =>
        services.GetRequiredService<SettingsViewModel>();
}
