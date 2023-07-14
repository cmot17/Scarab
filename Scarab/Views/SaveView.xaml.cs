using JetBrains.Annotations;
using Scarab.ViewModels;

namespace Scarab.Views;

[UsedImplicitly]
public partial class SaveView : View<SaveViewModel>
{
    public SaveView()
    {
        InitializeComponent();
    }
}