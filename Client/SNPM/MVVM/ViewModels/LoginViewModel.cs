using SNPM.Core;
using SNPM.Core.Interfaces;
using SNPM.MVVM.Views;
using SNPM.MVVM.Views.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SNPM.MVVM.ViewModels
{

    public delegate void OnLoginSucessful();
    public class LoginViewModel
    {
        public ILoginView? LoginView { get; set; }

        private IAccount PotentialAccount;
        public string Username { get => PotentialAccount.Username; set { PotentialAccount.Username = value; } }
        public string Password { get => PotentialAccount.Password; set { PotentialAccount.Password = value; } }

        public event OnLoginSucessful? LoginSuccessfulEvent;
        public ICommand LoginCommand { get; set; }

        public LoginViewModel()
        {
            PotentialAccount = new Account();

            Username = string.Empty;
            Password = string.Empty;

            LoginCommand = new RelayCommand(OnLoginAttempt);
        }

        private void OnLoginAttempt(object sender)
        {
            if (LoginView == null)
            {
                throw new Exception("Login attempted, but no LoginView is set.");
            }

            if (PotentialAccount.CheckIfCorrect())
            {
                LoginView.Hide();
                LoginSuccessfulEvent?.Invoke();
            }
            else
            {
                // TODO: (Przemek) make error
            }
        }
    }
}
