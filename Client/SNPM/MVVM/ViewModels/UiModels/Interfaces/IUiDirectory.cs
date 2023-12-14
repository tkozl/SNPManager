using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SNPM.MVVM.Models.UiModels.Interfaces
{
    public interface IUiDirectory : IDirectory, INotifyPropertyChanged
    {
        ObservableCollection<IUiDirectory> Children { get; }

        string OldName { get; }
    }
}
