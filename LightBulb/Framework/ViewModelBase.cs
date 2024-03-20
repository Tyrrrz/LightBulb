using CommunityToolkit.Mvvm.ComponentModel;

namespace LightBulb.Framework;

public abstract class ViewModelBase : ObservableObject
{
    protected void OnAllPropertiesChanged() => OnPropertyChanged(string.Empty);
}
