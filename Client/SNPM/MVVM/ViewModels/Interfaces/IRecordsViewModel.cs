using SNPM.Core;
using SNPM.Core.Interfaces;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SNPM.MVVM.ViewModels.Interfaces
{
    interface IRecordsViewModel
    {
        ObservableCollection<IRecord> Records { get; }

        IRecord? SelectedRecord { get; set; }

        ICommand NewRecordCommand { get; }

        ICommand RenameRecordCommand { get; }

        ICommand DeleteRecordCommand { get; }

        Task RefreshRecords(int directoryId);
    }
}
