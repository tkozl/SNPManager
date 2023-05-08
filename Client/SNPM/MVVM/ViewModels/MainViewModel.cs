using SNPM.Core;
using System;

namespace SNPM.MVVM.ViewModels
{
    class MainViewModel : ObservableObject
    {
        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        private object _recordsView;

        public object RecordsView
        {
            get { return _recordsView; }
            set {
                _recordsView = value;
                OnPropertyChanged();
            }
        }

        public string Title { get; }

        public MainViewModel()
        {
            Title = "Secure Network Password Manager";

            // Dependency injection here
            RecordsView = new RecordsViewModel();
        }
    }
}
