using SNPM.Core;
using SNPM.Core.Events;
using SNPM.Core.Interfaces;
using SNPM.MVVM.Models.UiModels.Interfaces;
using SNPM.MVVM.ViewModels.Interfaces;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SNPM.MVVM.ViewModels
{
    public class RecordsViewModel : ObservableObject, IRecordsViewModel
    {
        private readonly IProxyService proxyService;
        private readonly IDirectoryViewModel directoryViewModel;
        private readonly IRecordFormViewModel recordFormViewModel;

        public ObservableCollection<IUiRecord> Records { get; }

        public IUiRecord? SelectedRecord { get; set; }

        public ICommand NewRecordCommand { get; }

        public ICommand ModifyRecordCommand { get; }

        public ICommand DeleteRecordCommand { get; }

        public RecordsViewModel(IProxyService proxyService, IDirectoryViewModel directoryViewModel, IRecordFormViewModel recordFormViewModel)
        {
            this.proxyService = proxyService;
            this.directoryViewModel = directoryViewModel;
            this.recordFormViewModel = recordFormViewModel;

            recordFormViewModel.RecordCreatedEvent += OnRecordCreated;
            Records = new ObservableCollection<IUiRecord>();
            NewRecordCommand = new RelayCommand(CreateNewRecord);
            ModifyRecordCommand = new RelayCommand(ModifyRecord, CanModifyRecord);
            DeleteRecordCommand = new RelayCommand(DeleteRecord, CanDeleteRecord);
        }

        public async Task RefreshRecords(int directoryId)
        {
            var records = await proxyService.GetDirectoryRecords(directoryId);

            Records.Clear();
            Records.AddRange(records);
        }

        private async void CreateNewRecord(object sender)
        {
            if (directoryViewModel.SelectedNode == null)
            {
                return;
            }

            recordFormViewModel.OpenCreateDialog(directoryViewModel.SelectedNode.Id);
        }

        private async void ModifyRecord(object sender)
        {
            if (sender is IUiRecord uiRecord)
            {
                recordFormViewModel.OpenCreateDialog(uiRecord);
            }
        }

        private bool CanModifyRecord(object sender) => SelectedRecord != null;
        
        private async void DeleteRecord(object sender)
        {

        }

        private bool CanDeleteRecord(object sender) => SelectedRecord != null;

        private void OnRecordCreated(object? sender, System.EventArgs e)
        {
            if (e is RecordCreatedEventArgs recordCreatedEventArgs)
            {
                var createdRecord = recordCreatedEventArgs.Record;

                if (createdRecord.DirectoryId == directoryViewModel.SelectedNode.Id)
                {
                    Records.Add(createdRecord);
                }
            }
        }
    }
}
