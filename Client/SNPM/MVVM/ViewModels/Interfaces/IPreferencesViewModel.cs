using SNPM.Core.Interfaces;
using System;
using System.Collections.ObjectModel;

namespace SNPM.MVVM.ViewModels.Interfaces
{
    interface IPreferencesViewModel
    {
        public Action CloseAction { get; set; }

        public ObservableCollection<IOption> Options { get; }

        event PreferenceHandler PreferenceChanged;
    }
}
