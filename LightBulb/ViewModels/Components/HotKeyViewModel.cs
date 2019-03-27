using LightBulb.Models;
using Stylet;

namespace LightBulb.ViewModels.Components
{
    public partial class HotKeyViewModel : PropertyChangedBase
    {
        public HotKey Model { get; set; }
    }

    public partial class HotKeyViewModel
    {
        public static implicit operator HotKey(HotKeyViewModel viewModel) => viewModel.Model;
    }
}