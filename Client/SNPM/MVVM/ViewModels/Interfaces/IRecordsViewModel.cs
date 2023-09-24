using SNPM.MVVM.Models.UiModels.Interfaces;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SNPM.MVVM.ViewModels.Interfaces
{
    interface IRecordsViewModel
    {
        ObservableCollection<IUiRecord> Records { get; }

        IUiRecord? SelectedRecord { get; set; }

        ICommand NewRecordCommand { get; }

        ICommand ModifyRecordCommand { get; }

        ICommand DeleteRecordCommand { get; }

        Task RefreshRecords(int directoryId);
    }
}
