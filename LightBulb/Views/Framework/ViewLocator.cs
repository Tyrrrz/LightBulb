using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using CommunityToolkit.Mvvm.ComponentModel;
using LightBulb.ViewModels.Framework;

namespace LightBulb.Views.Framework;

public class ViewLocator : IDataTemplate
{
    public Control? TryResolveView(ViewModelBase viewModel)
    {
        var name = viewModel.GetType().FullName!.Replace("ViewModel", "View");

        var type = Type.GetType(name);
        if (type is null)
            return null;

        return Activator.CreateInstance(type) as Control;
    }

    bool IDataTemplate.Match(object? data) => data is ViewModelBase;

    Control? ITemplate<object?, Control?>.Build(object? data) =>
        data is ViewModelBase viewModel ? TryResolveView(viewModel) : null;
}
