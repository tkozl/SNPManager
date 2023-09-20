using SNPM.Core;
using SNPM.Core.Interfaces;
using SNPM.MVVM.ViewModels.Interfaces;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SNPM.MVVM.ViewModels
{
    public class RecordsViewModel : ObservableObject, IRecordsViewModel
    {
        private readonly IProxyService proxyService;

        public ObservableCollection<IRecord> Records { get; }

        public IRecord? SelectedRecord { get; set; }

        public ICommand NewRecordCommand => throw new System.NotImplementedException();

        public ICommand RenameRecordCommand => throw new System.NotImplementedException();

        public ICommand DeleteRecordCommand => throw new System.NotImplementedException();

        public RecordsViewModel(IProxyService proxyService)
        {
            this.proxyService = proxyService;

            Records = new ObservableCollection<IRecord>();
        }

        public async Task RefreshRecords(int directoryId)
        {
            var records = await proxyService.GetDirectoryRecords(directoryId);

            Records.Clear();
            Records.AddRange(records);
        }
    }
}
