using SNPM.MVVM.Models.Interfaces;
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
        private ObservableCollection<RelatedWindow> relatedWindows;
        private string note;
        private int directoryId;
        private ObservableCollection<IUiParameter> parameters;

        public int DirectoryId
        {
            get => directoryId;
            set
            {
                directoryId = value;
                OnPropertyChanged(nameof(DirectoryId));
            }
        }

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
                OnPropertyChanged(nameof(IsPasswordNotEmpty));
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

        public ObservableCollection<RelatedWindow> RelatedWindows
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

        public ObservableCollection<IUiParameter> Parameters
        {
            get => parameters;
            set
            {
                parameters = value;
                OnPropertyChanged(nameof(Parameters));
            }
        }

        public bool IsPasswordNotEmpty => Password?.Length > 0;

        public bool IsExpired => Lifetime.ToUniversalTime() < DateTime.UtcNow;

        public event PropertyChangedEventHandler? PropertyChanged;

        public UiRecord() : base()
        {
            RelatedWindows = new ObservableCollection<RelatedWindow>();
            Parameters = new ObservableCollection<IUiParameter>();
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

            this.RelatedWindows = new ObservableCollection<RelatedWindow>();
            this.Parameters = new ObservableCollection<IUiParameter>();
            foreach (var windowName in domainRecord.RelatedWindows)
            {
                this.RelatedWindows.Add(new RelatedWindow(windowName));
            }

            foreach (var parameter in domainRecord.Parameters)
            {
                this.Parameters.Add(new UiParameter(parameter.Name, parameter.Value));
            }

            this.Note = domainRecord.Note;
        }

        public void Clear()
        {
            this.EntryId = 0;
            this.DirectoryId = 0;
            this.DirectoryName = string.Empty;
            this.Name = string.Empty;
            this.Username = string.Empty;
            this.Password = string.Empty;
            this.Lifetime = DateTime.UtcNow;
            this.RelatedWindows.Clear();
            this.Parameters.Clear();
            this.Note = string.Empty;
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
            this.RelatedWindows = new ObservableCollection<RelatedWindow>(uiRecord.RelatedWindows);
            this.Parameters = new ObservableCollection<IUiParameter>(uiRecord.Parameters);
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
