using SNPM.Core;
using SNPM.Core.Api;
using SNPM.Core.Interfaces;
using SNPM.Core.Interfaces.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SNPM.MVVM.Models
{
    public class ApplicationLogic
    {
        private Window MainView;

        private IApiService ApiService;
        private IPasswordVerifier PasswordVerifierService;
        public ApplicationLogic()
        {
            MainView = new Views.MainView();

            ApiService = new PlaceholderApiService();

            PasswordVerifierService = new PasswordVerifier(ApiService.GetRemoteVerifier(), HashType.SHA256);
        }

        public void Initialize()
        {
            Window loginView = new Views.LoginView(() => { 
                MainView.Show();
                
            });
            loginView.Show();
        }
    }
}
