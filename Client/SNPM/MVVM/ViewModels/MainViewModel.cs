using Microsoft.Extensions.DependencyInjection;
using SNPM.Core;
using SNPM.MVVM.ViewModels.Interfaces;
using SNPM.MVVM.Views;
using System;
using System.ComponentModel;
using System.Windows.Input;
using SNPM.MVVM.Models.Interfaces;
using System.Windows.Interop;

namespace SNPM.MVVM.ViewModels
{
    class MainViewModel : ObservableObject, IMainViewModel
    {
        private IRecordsViewModel recordsViewModel;

        public IRecordsViewModel RecordsViewModel
        {
            get { return recordsViewModel; }
            set {
                recordsViewModel = value;
                OnPropertyChanged();
            }
        }

        private IPreferencesViewModel PreferencesViewModel;

        public IDirectoryViewModel DirectoryTreeViewModel { get; private set; }


        private PreferencesView _preferencesView;

        public PreferencesView PreferencesView
        {
            get { return _preferencesView; }
            set { _preferencesView = value; }
        }

        public IActivityViewModel ActivityViewModel { get; set; }

        public IntPtr MainWindowHandle { get; }

        public string Title { get; }
        public Action CloseAction { get; set; }

        public string StatusMessage => ActivityViewModel.StatusMessage;

        public ICommand PreferencesCommand { get; set; }

        private MainView mainView;
        private readonly IServiceProvider serviceProvider;
        private readonly IProxyService proxyService;

        public MainViewModel(IServiceProvider serviceProvider, IProxyService proxyService)
        {
            this.serviceProvider = serviceProvider;
            this.proxyService = proxyService;

            Title = "Secure Network Password Manager";
            CloseAction = new Action(OnClose);
            PreferencesCommand = new RelayCommand(OnPreferenceOpen);

            RecordsViewModel = serviceProvider.GetService<IRecordsViewModel>() ?? throw new Exception("ViewModel not registered");
            PreferencesViewModel = serviceProvider.GetService<IPreferencesViewModel>() ?? throw new Exception("ViewModel not registered");
            DirectoryTreeViewModel = serviceProvider.GetService<IDirectoryViewModel>() ?? throw new Exception("ViewModel not registered");
            ActivityViewModel = serviceProvider.GetService<IActivityViewModel>() ?? throw new Exception("ViewModel not registered");

            PreferencesView = new PreferencesView
            {
                DataContext = PreferencesViewModel
            };
            PreferencesViewModel.CloseAction = new Action(() =>
            {
                PreferencesView.Hide();
            });

            mainView = new MainView()
            {
                DataContext = this
            };

            MainWindowHandle = new WindowInteropHelper(mainView).Handle;
        }

        public void SubscribeToPreferenceUpdate(PreferenceHandler handler)
        {
            PreferencesViewModel.PreferenceChanged += handler;
            DirectoryTreeViewModel.PropertyChanged += OnDirectoryTreePropertyChanged;
        }

        private void OnDirectoryTreePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedNode" && DirectoryTreeViewModel.SelectedNode != null)
            {
                RecordsViewModel.RefreshRecords(DirectoryTreeViewModel.SelectedNode.Id);
            }
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

        public void ShowView()
        {
            mainView.Show();

            RefreshRecords();
        }

        public void HideView()
        {
            mainView.Hide();
        }

        private void RefreshRecords()
        {
            DirectoryTreeViewModel.RebuildDirectoryTree();
            ActivityViewModel.Refresh();
        }
    }
}
