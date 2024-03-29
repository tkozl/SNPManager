﻿using SNPM.MVVM.Models.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SNPM.MVVM.Models.UiModels.Interfaces
{
    public interface IUiRecord : INotifyPropertyChanged, IErrorContainer
    {
        int DirectoryId { get; set; }

        string Name { get; set; }

        string Username { get; set; }

        string Password { get; set; }

        ObservableCollection<RelatedWindow> RelatedWindows { get; set; }

        string Note { get; set; }

        string DirectoryName { get; set; }

        DateTime Lifetime { get; set; }

        int EntryId { get; }

        bool IsExpired { get; }
        bool IsPasswordNotEmpty { get; }
        ObservableCollection<IUiParameter> Parameters { get; set; }

        void Clear();

        void CloneProperties(IUiRecord uiRecord);
    }
}
