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
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown(); // TODO: (PrzemeK) Should call ApplicationLogic
        }

        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {

        }
    }
}
