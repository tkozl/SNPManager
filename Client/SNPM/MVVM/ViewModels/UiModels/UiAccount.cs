using SNPM.MVVM.Models.UiModels.Interfaces;
using System.Collections.ObjectModel;

namespace SNPM.MVVM.Models.UiModels
{
    internal class UiAccount : IUiAccount
    {
        public bool Is2FaRequired { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }

        public ObservableCollection<string> Errors { get; private set; }

        public UiAccount()
        {
            Username = "";
            Password = "";
            Errors = new();

            Is2FaRequired = false;
        }

        // TODO: Perform only simple sanity checks on UI layer, move rest to BL
        public bool CheckIfCorrect()
        {
            return true;
            return Username != null && Username.Length > 5 && Password != null && Password.Length > 10;
        }
    }
}
