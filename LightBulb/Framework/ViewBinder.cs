using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace LightBulb.Framework;

public partial class ViewBinder
{
    public Control? TryBindView(ViewModelBase viewModel)
    {
        var name = viewModel
            .GetType()
            .FullName
            ?.Replace("ViewModel", "View", StringComparison.Ordinal);

        if (string.IsNullOrWhiteSpace(name))
            return null;

        var type = Type.GetType(name);
        if (type is null)
            return null;

        if (Activator.CreateInstance(type) is not Control control)
            return null;
        
        control.DataContext ??= viewModel;
        
        return control;
    }
}

public partial class ViewBinder : IDataTemplate
{
    bool IDataTemplate.Match(object? data) => data is ViewModelBase;

    Control? ITemplate<object?, Control?>.Build(object? data) =>
        data is ViewModelBase viewModel ? TryBindView(viewModel) : null;
}
