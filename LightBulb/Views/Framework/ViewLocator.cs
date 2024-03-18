using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LightBulb.Views.Framework;

public class ViewLocator : IDataTemplate
{
    public bool Match(object? data) => data is ObservableObject;

    public Control? Build(object? data)
    {
        if (data is null)
        {
            return null;
        }

        var name = data.GetType().FullName!.Replace("ViewModel", "View");
        var type = Type.GetType(name);

        if (type is null)
            return new TextBlock { Text = "Not Found: " + name };

        return (Control)Activator.CreateInstance(type)!;
    }
}
