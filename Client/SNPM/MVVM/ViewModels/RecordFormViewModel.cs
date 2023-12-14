using SNPM.Core;
using SNPM.Core.BusinessLogic.Interfaces;
using SNPM.Core.Events;
using SNPM.Core.Extensions;
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
        private readonly IDialogService dialogService;
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

        public RecordFormViewModel(IProxyService proxyService, IDialogService dialogService)
        {
            View = new RecordFormView()
            {
                DataContext = this,
            };

            CreatedRecord = new UiRecord();

            CancelCommand = new RelayCommand(Cancel);
            ConfirmCommand = new RelayCommand(Create, CanCreate);
            AddRelatedWindowCommand = new RelayCommand(AddEmptyWindow);
            AddParameterCommand = new RelayCommand(AddEmptyParamter);
            this.proxyService = proxyService;
            this.dialogService = dialogService;
            Directories = new ObservableCollection<IUiDirectory>();
        }

        public RecordFormView View { get; }

        public ICommand CancelCommand { get; }

        public ICommand ConfirmCommand { get; }

        public ICommand AddRelatedWindowCommand { get; }

        public ICommand AddParameterCommand { get; }

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
            
            CreatedRecord.Clear();
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
            var passwordQuality = await proxyService.VerifyPassword(CreatedRecord.Password);

            var isStrong = !passwordQuality.GetUniqueFlags().Any();
            var errors = passwordQuality.GetUniqueFlags().Select(x => x.ToString());

            if (isStrong)
            {
                var record = await proxyService.CreateRecord(CreatedRecord, currentId);
                RecordCreatedEvent.Invoke(this, new RecordCreatedEventArgs(record));
                this.HideView();
            }
            else
            {
                await dialogService.CreateErrorDialog("Password too weak!", errors.ToList());
            }
        }

        private void AddEmptyWindow(object sender)
        {
            if (CreatedRecord?.RelatedWindows != null)
            {
                CreatedRecord.RelatedWindows.Add(new RelatedWindow());
            }
        }

        private void AddEmptyParamter(object sender)
        {
            if (CreatedRecord?.Parameters != null)
            {
                CreatedRecord.Parameters.Add(new UiParameter(string.Empty, string.Empty));
            }
        }
    }
}
