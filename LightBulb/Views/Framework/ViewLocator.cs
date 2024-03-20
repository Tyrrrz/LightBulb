using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using LightBulb.ViewModels.Framework;

namespace LightBulb.Views.Framework;

public partial class ViewLocator
{
    public Control? TryResolveView(ViewModelBase viewModel)
    {
        var name = viewModel.GetType().FullName?.Replace("ViewModel", "View", StringComparison.Ordinal);
        if (string.IsNullOrWhiteSpace(name))
            return null;

        var type = Type.GetType(name);
        if (type is null)
            return null;

        return Activator.CreateInstance(type) as Control;
    }
}

public partial class ViewLocator : IDataTemplate
{
    bool IDataTemplate.Match(object? data) => data is ViewModelBase;

    Control? ITemplate<object?, Control?>.Build(object? data) =>
        data is ViewModelBase viewModel ? TryResolveView(viewModel) : null;   
}