using Microsoft.Extensions.DependencyInjection;
using SNPM.Core;
using SNPM.Core.Interfaces;
using SNPM.MVVM.ViewModels.Interfaces;
using SNPM.MVVM.Views;
using SNPM.MVVM.Views.Interfaces;
using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Threading.Tasks;

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

        private IPreferencesViewModel PreferencesViewModel;

        public IDirectoryViewModel DirectoryViewModel { get; private set; }

        private PreferencesView _preferencesView;

        public PreferencesView PreferencesView
        {
            get { return _preferencesView; }
            set { _preferencesView = value; }
        }

        public string Title { get; }
        public Action CloseAction { get; set; }

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

            RecordsView = serviceProvider.GetService<IRecordsViewModel>() ?? throw new Exception("ViewModel not registered");
            PreferencesViewModel = serviceProvider.GetService<IPreferencesViewModel>() ?? throw new Exception("ViewModel not registered");
            DirectoryViewModel = serviceProvider.GetService<IDirectoryViewModel>() ?? throw new Exception("ViewModel not registered");
            DirectoryViewModel.View = serviceProvider.GetService<IDirectoryView>() ?? throw new Exception("View not registered");
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

        public void ShowView()
        {
            mainView.Show();

            RefreshRecords();
        }

        public void HideView()
        {
            mainView.Hide();
        }

        private async Task RefreshRecords()
        {
            DirectoryViewModel.Directories.Clear();

            var directories = await proxyService.GetDirectories(1);
        }
    }
}
