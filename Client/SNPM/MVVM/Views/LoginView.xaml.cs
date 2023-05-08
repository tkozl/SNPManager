using SNPM.MVVM.ViewModels;
using SNPM.MVVM.Views.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SNPM.MVVM.Views
{
    /// <summary>
    /// Logika interakcji dla klasy LoginView.xaml
    /// </summary>
    public partial class LoginView : Window, ILoginView
    {
        public LoginView(OnLoginSucessful loginSuccessfulHandler)
        {
            InitializeComponent();

            //DataContextChanged += OnDataContextChanged;
            if (DataContext is LoginViewModel viewModel)
            {
                viewModel.LoginView = this;
                viewModel.LoginSuccessfulEvent += loginSuccessfulHandler;
            }
        }

        //private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    ((LoginViewModel)sender).LoginView = this;
        //}
    }
}
