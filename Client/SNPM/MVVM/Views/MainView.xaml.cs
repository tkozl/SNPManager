using SNPM.MVVM.Views.Interfaces;
using System.Windows;

namespace SNPM.MVVM.Views
{
    public partial class MainView : Window, IMainView
    {
        public MainView()
        {
            InitializeComponent();
            //var viewModel = new MainViewModel();
            //this.DataContext = viewModel;
            this.Closing += OnWindowClosing;
        }

        private void OnWindowClosing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
