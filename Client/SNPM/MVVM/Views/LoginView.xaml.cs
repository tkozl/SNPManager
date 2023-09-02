using SNPM.MVVM.Views.Interfaces;
using System;
using System.Windows;

namespace SNPM.MVVM.Views
{
    /// <summary>
    /// Logika interakcji dla klasy LoginView.xaml
    /// </summary>
    public partial class LoginView : Window, ILoginView
    {
        public LoginView()
        {
            InitializeComponent();

            //DataContextChanged += OnDataContextChanged;
            //if (DataContext is LoginViewModel viewModel)
            //{
            //    viewModel.LoginView = this;
            //    viewModel.LoginSuccessfulEvent += loginSuccessfulHandler;
            //}
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown(); // TODO: (PrzemeK) Should call ApplicationLogic
        }

        //private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    ((LoginViewModel)sender).LoginView = this;
        //}
    }
}
