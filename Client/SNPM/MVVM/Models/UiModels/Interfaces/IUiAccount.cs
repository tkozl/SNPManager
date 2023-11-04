using System.Collections.ObjectModel;

namespace SNPM.MVVM.Models.UiModels.Interfaces
{
    public interface IUiAccount
    {
        bool Is2FaRequired { get; set; }

        string Username { get; set; }

        string Password { get; set; }

        ObservableCollection<string> Errors { get; }

        bool CheckIfCorrect();
    }
}
