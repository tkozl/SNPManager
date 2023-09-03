using Microsoft.Extensions.DependencyInjection;
using SNPM.Core.Interfaces;
using SNPM.Core.Interfaces.Api;
using SNPM.MVVM.ViewModels;
using SNPM.MVVM.ViewModels.Interfaces;
using System;
using static SNPM.Core.Interfaces.IApplicationLogic;

namespace SNPM.MVVM.Models
{
    public class ApplicationLogic : IApplicationLogic
    {
        private readonly IApiService apiService;
        private readonly IProxyService proxyService;
        private readonly IPasswordVerifier? passwordVerifierService;

        private IMainViewModel mainViewModel;
        private ILoginViewModel loginViewModel;
        
        public event OptionChanged OnOptionChange;

        public ApplicationLogic(
            IPasswordVerifier passwordVerifierService,
            IApiService apiService,
            IProxyService proxyService,
            IDialogService dialogService,
            IServiceProvider serviceProvider)
        {
            this.apiService = apiService;
            this.proxyService = proxyService;
            this.passwordVerifierService = passwordVerifierService;

            mainViewModel = serviceProvider.GetService<IMainViewModel>() ?? throw new Exception("ViewModel not registered");
            mainViewModel.SubscribeToPreferenceUpdate(OnPreferenceUpdate);

            loginViewModel = serviceProvider.GetService<ILoginViewModel>() ?? throw new Exception("LoginViewModel not registered");
            loginViewModel.LoginSuccessfulEvent += mainViewModel.ShowView;
            loginViewModel.ShowView();

            OnExit = new Action(Shutdown);
        }

        public Action OnExit { get; }

        public void Initialize()
        {
            //var loginVm = new LoginViewModel(proxyService);

            //Window loginView = new Views.LoginView(() => { MainView.Show(); })
            //{
            //    DataContext = loginVm
            //};
            //loginView.Show();
            //loginViewModel = new 
        }

        private void Shutdown()
        {
            // TODO: (Przemek) Clear stuff before we exit
            System.Windows.Application.Current.Shutdown();
        }

        private void OnPreferenceUpdate(string PropertyName, object NewValue)
        {
            var option = Enum.Parse<ChangeableOption>(PropertyName);

            switch (option)
            {
                case ChangeableOption.TextSize:
                    OnOptionChange.Invoke(option, NewValue);
                    break;
                case ChangeableOption.DarkMode:

                    break;
                default:
                    throw new Exception("Option not handled");
            }
        }
    }
}
