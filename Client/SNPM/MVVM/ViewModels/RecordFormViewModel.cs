using SNPM.Core;
using SNPM.Core.Events;
using SNPM.Core.Interfaces;
using SNPM.MVVM.Models.UiModels;
using SNPM.MVVM.Models.UiModels.Interfaces;
using SNPM.MVVM.ViewModels.Interfaces;
using SNPM.MVVM.Views;
using System;
using System.Windows.Input;

namespace SNPM.MVVM.ViewModels
{
    internal class RecordFormViewModel : IRecordFormViewModel
    {
        private readonly IProxyService proxyService;
        private int? currentId;

        public event EventHandler RecordCreatedEvent;

        public IUiRecord CreatedRecord { get; private set; }

        public RecordFormViewModel(IProxyService proxyService)
        {
            View = new RecordFormView()
            {
                DataContext = this,
            };

            CreatedRecord = new UiRecord();

            CancelCommand = new RelayCommand(Cancel);
            ConfirmCommand = new RelayCommand(Create, CanCreate);
            AddRelatedWindowCommand = new RelayCommand(AddEmptyWindow);
            this.proxyService = proxyService;
        }

        public RecordFormView View { get; }

        public ICommand CancelCommand { get; }

        public ICommand ConfirmCommand { get; }

        public ICommand AddRelatedWindowCommand { get; }

        public void HideView()
        {
            View.Hide();
        }

        public void ShowView()
        {
            View.Show();
        }

        public void OpenCreateDialog(int directoryId)
        {
            CreatedRecord.DirectoryId = directoryId;
            CreatedRecord.Lifetime = DateTime.UtcNow;
            currentId = null;
            this.ShowView();
        }

        public void OpenCreateDialog(IUiRecord uiRecord)
        {
            CreatedRecord.CloneProperties(uiRecord);
            currentId = uiRecord.EntryId;
            this.ShowView();
        }

        private void Cancel(object sender)
        {
            this.HideView();
        }

        private bool CanCreate(object sender)
        {
            return true;
        }

        private async void Create(object sender)
        {
            var record = await proxyService.CreateRecord(CreatedRecord, currentId);
            RecordCreatedEvent.Invoke(this, new RecordCreatedEventArgs(record));
            this.HideView();
        }

        private void AddEmptyWindow(object sender)
        {
            if (CreatedRecord?.RelatedWindows != null)
            {
                CreatedRecord.RelatedWindows.Add(string.Empty);
            }
        }
    }
}
