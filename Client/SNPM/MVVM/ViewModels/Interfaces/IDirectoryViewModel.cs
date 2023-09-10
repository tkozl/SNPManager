using SNPM.MVVM.Models.UiModels.Interfaces;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SNPM.MVVM.ViewModels.Interfaces
{
    interface IDirectoryViewModel : IViewModel
    {
        IUiDirectory DirectoryTree { get; }

        IUiDirectory? SelectedNode { get; set; }

        ObservableCollection<IUiDirectory> RootNodes { get; }

        Task RebuildDirectoryTree();

        ICommand NewDirectoryCommand { get; }

        ICommand RenameDirectoryCommand { get; }

        ICommand DeleteDirectoryCommand { get; }
    }
}
