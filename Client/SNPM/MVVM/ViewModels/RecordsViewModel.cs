using SNPM.Core;
using SNPM.MVVM.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SNPM.MVVM.ViewModels
{
    public class RecordsViewModel : ObservableObject
    {
        public RelayCommand RowClick { get; set; }

        private ObservableCollection<ObservableObject> _records;

        public ObservableCollection<ObservableObject> Records
        {
            get => this._records;
            set { 
                _records = value;
            }
        }

        public RecordsViewModel()
        {
            Records = new();
            Populate();

            RowClick = new RelayCommand(Row_MouseClick);
        }

        private void Populate()
        {
            Records.Add(new Record("asd", "gdzies", "user", "lol"));
            Records.Add(new Record("aaad", "gdzies", "user", "lol"));
            Records.Add(new Record("cxz", "gdzies", "user", "lol"));
            Records.Add(new Record("asd", "gdzies", "user", "lol"));
        }

        private void Row_MouseClick(object sender)
        {

        }
    }
}
