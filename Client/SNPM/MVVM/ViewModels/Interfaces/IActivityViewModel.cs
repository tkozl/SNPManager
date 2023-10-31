using SNPM.MVVM.Models.UiModels;
using System.Collections.ObjectModel;

namespace SNPM.MVVM.ViewModels.Interfaces
{
    public interface IActivityViewModel : IViewModel
    {
        ObservableCollection<LoginAttempt> ServerMessages { get; set; }

        void Refresh();
    }
}
