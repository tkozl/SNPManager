using SNPM.MVVM.Models.UiModels;
using SNPM.MVVM.Views.Interfaces;
using System.Collections.ObjectModel;

namespace SNPM.MVVM.ViewModels.Interfaces
{
    interface IDirectoryViewModel : IViewModel
    {
        IDirectoryView View { get; set; }

        ObservableCollection<Directory> Directories { get; }
    }
}
