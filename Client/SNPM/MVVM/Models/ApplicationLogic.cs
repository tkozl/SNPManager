using SNPM.Core;
using SNPM.Core.Interfaces;
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

        private IServerConnection serverConnection;
        public ApplicationLogic()
        {
            MainView = new Views.MainView();

            serverConnection = new ServerConnection();
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
