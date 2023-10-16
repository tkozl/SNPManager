using Microsoft.Extensions.DependencyInjection;
using SNPM.Core;
using SNPM.Core.BusinessLogic.Interfaces;
using SNPM.MVVM.Models.Interfaces;
using SNPM.MVVM.Models.UiModels;
using SNPM.MVVM.Models.UiModels.Interfaces;
using SNPM.MVVM.ViewModels.Interfaces;
using SNPM.MVVM.Views;
using SNPM.MVVM.Views.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SNPM.MVVM.ViewModels
{
    public class LoginViewModel : ILoginViewModel
    {
        public Window LoginView { get; set; }

        private IUiAccount Account;
        private readonly IProxyService proxyService;
        private readonly IDialogService dialogService;

        public string Username { get => Account.Username; set { Account.Username = value; } }
        public string Password { get => Account.Password; set { Account.Password = value; } }

        public ObservableCollection<string> Errors => Account.Errors;

        public event OnLoginSucessful? LoginSuccessfulEvent;
        public ICommand LoginCommand { get; set; }

        public ICommand RegisterCommand { get; set; }

        public LoginViewModel(IProxyService proxyService, IDialogService dialogService, IServiceProvider serviceProvider)
        {
            this.proxyService = proxyService;
            this.dialogService = dialogService;
            Account = new UiAccount();

            //Username = $"testujeRejestracje@{DateTime.UtcNow.ToString("HH.mm.ss")}";
            //Password = "Abcdefg1234567@";
            Username = "user@example.com";
            Password = "securepassword";
            //Password = new string('a', 10000);

            LoginCommand = new RelayCommand(OnLoginAttempt);
            RegisterCommand = new RelayCommand(OnRegisterAttempt);

            LoginView = serviceProvider.GetService<ILoginView>() as Window ?? throw new Exception("LoginView not registered");
            LoginView.DataContext = this;
        }

        private async void OnLoginAttempt(object sender)
        {
            if (Account.CheckIfCorrect())
            {
                await proxyService.Login(Account);

                if (!Account.Errors.Any())
                {
                    LoginView.Hide();
                    LoginSuccessfulEvent?.Invoke();
                }
                else
                {
                    await dialogService.CreateErrorDialog("Login failed", Account.Errors);
                }
            }
        }

        private async void OnRegisterAttempt(object sender)
        {
            if (Account.CheckIfCorrect())
            {
                await proxyService.CreateAccount(Account);

                if (!Account.Errors.Any())
                {
                    string mainMessage = "Account creation succeeded";
                    string supportiveMessage = "You can now login.";
                    string affirmativeMessage = "OK";

                    await dialogService.CreateDialogWindow(mainMessage, supportiveMessage, affirmativeMessage);
                }
                else
                {
                    await dialogService.CreateErrorDialog("Registration failed", Account.Errors);
                }
            }
        }

        public void ShowView()
        {
            LoginView.Show();
        }

        public void HideView()
        {
            LoginView.Hide();
        }
    }
}
