using System.Collections.ObjectModel;

namespace SNPM.MVVM.Models.UiModels.Interfaces
{
    public interface IUiAccount
    {
        string Username { get; set; }

        string Password { get; set; }

        ObservableCollection<string> Errors { get; }

        bool CheckIfCorrect();
    }
}
