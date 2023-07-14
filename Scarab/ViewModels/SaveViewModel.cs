using Scarab.Models;

namespace Scarab.ViewModels;

public class SaveViewModel : ViewModelBase
{
    public SaveViewModel(SaveGameData[] saves)
    {
        Saves = saves;
    }

    public SaveGameData[] Saves { get; set; }
}