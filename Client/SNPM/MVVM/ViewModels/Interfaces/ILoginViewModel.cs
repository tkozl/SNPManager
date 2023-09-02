using System.Collections.ObjectModel;

namespace SNPM.MVVM.ViewModels.Interfaces
{
    public delegate void OnLoginSucessful();
    public interface ILoginViewModel : IViewModel
    {
        ObservableCollection<string> Errors { get; }

        public event OnLoginSucessful? LoginSuccessfulEvent;
    }
}