using SNPM.Core;
using SNPM.MVVM.ViewModels.Interfaces;
using SNPM.MVVM.Views;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace SNPM.MVVM.ViewModels
{
    class MainViewModel : ObservableObject, IMainViewModel
    {
        private object _recordsView;

        public object RecordsView
        {
            get { return _recordsView; }
            set {
                _recordsView = value;
                OnPropertyChanged();
            }
        }

        private PreferencesViewModel PreferencesViewModel;

        private PreferencesView _preferencesView;

        public PreferencesView PreferencesView
        {
            get { return _preferencesView; }
            set { _preferencesView = value; }
        }

        public string Title { get; }
        public Action CloseAction { get; set; }

        public ICommand PreferencesCommand { get; set; }

        public MainViewModel()
        {
            Title = "Secure Network Password Manager";

            // Dependency injection here
            RecordsView = new RecordsViewModel();
            CloseAction = new Action(OnClose);
            PreferencesViewModel = new PreferencesViewModel();
            PreferencesView = new PreferencesView
            {
                DataContext = PreferencesViewModel
            };
            PreferencesViewModel.CloseAction = new Action(() =>
            {
                PreferencesView.Hide();
            });
            PreferencesCommand = new RelayCommand(OnPreferenceOpen);
        }

        public void SubscribeToPreferenceUpdate(PreferenceHandler handler)
        {
            PreferencesViewModel.PreferenceChanged += handler;
        }

        void OnClose()
        {
            // TODO: (Przemek) Application closing down, clear temp files. This should be in application logic? piwo
            System.Windows.Application.Current.Shutdown();
        }

        private void OnPreferenceOpen(object sender)
        {
            PreferencesView.Show();
        }
    }
}
