using Microsoft.Extensions.DependencyInjection;
using SNPM.Core;
using SNPM.Core.Api;
using SNPM.Core.Interfaces;
using SNPM.Core.Interfaces.Api;
using SNPM.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static SNPM.Core.Interfaces.IApplicationLogic;

namespace SNPM.MVVM.Models
{
    public class ApplicationLogic : IApplicationLogic
    {
        private Window MainView;
        private IApiService ApiService;
        private IPasswordVerifier? PasswordVerifierService;
        private ServiceProvider ServiceProvider;

        public event OptionChanged OnOptionChange;

        public ApplicationLogic(IPasswordVerifier PasswordVerifierService, IApiService ApiService)
        {
            var mainVm = new MainViewModel();
            mainVm.SubscribeToPreferenceUpdate(OnPreferenceUpdate);

            MainView = new Views.MainView()
            {
                DataContext = mainVm
            }; // TODO: (Przemek) Need a dependency injection class for views and viewmodels
            this.ApiService = ApiService;
            this.PasswordVerifierService = PasswordVerifierService;

            OnExit = new Action(Shutdown);
        }

        public Action OnExit { get; }

        public void Initialize()
        {
            Window loginView = new Views.LoginView(() => { 
                MainView.Show();
                
            });
            loginView.Show();
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
