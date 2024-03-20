using LightBulb.ViewModels.Dialogs;
using LightBulb.Views.Framework;

namespace LightBulb.Views.Dialogs;

public partial class MessageBoxView : ViewModelAwareUserControl<MessageBoxViewModel>
{
    public MessageBoxView() => InitializeComponent();
}
