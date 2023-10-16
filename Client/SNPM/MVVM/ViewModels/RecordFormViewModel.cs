using SNPM.Core;
using SNPM.Core.Events;
using SNPM.MVVM.Models.Interfaces;
using SNPM.MVVM.Models.UiModels;
using SNPM.MVVM.Models.UiModels.Interfaces;
using SNPM.MVVM.ViewModels.Interfaces;
using SNPM.MVVM.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SNPM.MVVM.ViewModels
{
    internal class RecordFormViewModel : IRecordFormViewModel
    {
        private readonly IProxyService proxyService;
        private int? currentId;
        private IUiDirectory selectedDirectory;

        public event EventHandler RecordCreatedEvent;

        public IUiRecord CreatedRecord { get; private set; }

        public ObservableCollection<IUiDirectory> Directories
        {
            get;
            set;
        }

        public IUiDirectory SelectedDirectory
        {
            get => selectedDirectory;
            set
            {
                selectedDirectory = value;
                if (value != null)
                {
                    CreatedRecord.DirectoryId = value.Id;
                }
            }
        }

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

            Directories = new ObservableCollection<IUiDirectory>();
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
            ReloadDirectories().Await();

            SelectedDirectory = Directories.First(x => x.Id == directoryId);

            CreatedRecord.DirectoryId = directoryId;
            CreatedRecord.Lifetime = DateTime.UtcNow;
            currentId = null;
            this.ShowView();
        }

        public void OpenCreateDialog(IUiRecord uiRecord)
        {
            ReloadDirectories().Await();

            CreatedRecord.CloneProperties(uiRecord);
            currentId = uiRecord.EntryId;

            SelectedDirectory = Directories.First(x => x.Id == uiRecord.DirectoryId);

            this.ShowView();
        }

        private async Task ReloadDirectories()
        {
            var dirs = await proxyService.GetDirectories(0, false);

            Directories.Clear();
            foreach (var dir in dirs)
            {
                Directories.Add(dir);
            }
        }

        private void Cancel(object sender)
        {
            this.HideView();
        }

        private bool CanCreate(object sender)
        {
            return CreatedRecord.Username != string.Empty
                   && CreatedRecord.Name != string.Empty
                   && (CreatedRecord.Lifetime > DateTime.UtcNow || !CreatedRecord.IsPasswordNotEmpty);
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
                CreatedRecord.RelatedWindows.Add(new RelatedWindow());
            }
        }
    }
}
