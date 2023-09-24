using SNPM.Core;
using SNPM.Core.Interfaces;
using SNPM.MVVM.Models.UiModels.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SNPM.MVVM.Models.UiModels
{
    internal class UiRecord : ErrorContainer, IUiRecord
    {
        private string name;
        private string directoryName;
        private string username;
        private string password;
        private DateTime lifetime;
        private ObservableCollection<string> relatedWindows;
        private string note;

        public int DirectoryId { get; set; }

        public int EntryId { get; set; }

        public string DirectoryName
        {
            get => directoryName;
            set
            {
                directoryName = value;
                OnPropertyChanged(nameof(DirectoryName));
            }
        }

        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Username
        {
            get => username;
            set
            {
                username = value;
                OnPropertyChanged(nameof(Username));
            }
        }
        public string Password
        {
            get => password;
            set
            {
                password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public DateTime Lifetime
        {
            get => lifetime;
            set
            {
                lifetime = value;
                OnPropertyChanged(nameof(Lifetime));
            }
        }

        public ObservableCollection<string> RelatedWindows
        {
            get => relatedWindows;
            set
            {
                relatedWindows = value;
                OnPropertyChanged(nameof(RelatedWindows));
            }
        }

        public string Note
        {
            get => note;
            set
            {
                note = value;
                OnPropertyChanged(nameof(Note));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public UiRecord() : base()
        {
            RelatedWindows = new ObservableCollection<string>();
        }

        public UiRecord(IRecord domainRecord)
        {
            this.EntryId = domainRecord.EntryId;
            this.DirectoryId = domainRecord.DirectoryId;
            this.DirectoryName = domainRecord.DirectoryName;
            this.Name = domainRecord.Name;
            this.Username = domainRecord.Username;
            this.Password = domainRecord.Password;
            this.Lifetime = domainRecord.Lifetime;
            this.RelatedWindows = new ObservableCollection<string>(domainRecord.RelatedWindows);
            this.Note = domainRecord.Note;
        }

        public void Clear()
        {
            this.Password = string.Empty;
        }

        public void CloneProperties(IUiRecord uiRecord)
        {
            this.EntryId = uiRecord.EntryId;
            this.DirectoryId = uiRecord.DirectoryId;
            this.DirectoryName = uiRecord.DirectoryName;
            this.Name = uiRecord.Name;
            this.Username = uiRecord.Username;
            this.Password = uiRecord.Password;
            this.Lifetime = uiRecord.Lifetime;
            this.RelatedWindows = new ObservableCollection<string>(uiRecord.RelatedWindows);
            this.Note = uiRecord.Note;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
