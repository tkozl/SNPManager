using SNPM.Core.Options;
using System;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace SNPM.MVVM.ViewModels.Interfaces
{
    interface IPreferencesViewModel
    {
        public Action CloseAction { get; set; }

        public ObservableCollection<IOption> Options { get; }

        bool Is2FaActive { get; set; }

        string Label2Fa { get; }

        DrawingImage Image { get; set; }

        event PreferenceHandler PreferenceChanged;
    }
}
