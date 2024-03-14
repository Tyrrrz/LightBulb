using CommunityToolkit.Mvvm.ComponentModel;
using LightBulb.ViewModels;

namespace LightBulb;

public class AppViewModel : ObservableObject
{
    public MainViewModel Main { get; }
}