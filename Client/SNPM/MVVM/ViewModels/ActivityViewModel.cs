using SNPM.MVVM.Models.Interfaces;
using SNPM.MVVM.Models.UiModels;
using SNPM.MVVM.Models.UiModels.Interfaces;
using SNPM.MVVM.ViewModels.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SNPM.MVVM.ViewModels
{
    internal class ActivityViewModel : IActivityViewModel
    {
        public ObservableCollection<LoginAttempt> ServerMessages { get; set; }

        private readonly IProxyService proxyService;

        public ActivityViewModel(IProxyService proxyService)
        {
            this.proxyService = proxyService;

            ServerMessages = new ObservableCollection<LoginAttempt>();
            Refresh();
        }

        public void HideView()
        {
        }

        public void ShowView()
        {
        }

        public void Refresh()
        {
            var accountActivity = proxyService.GetAccountActivity();

            if (accountActivity != null)
            {
                ServerMessages.Clear();

                ServerMessages.AddRange(accountActivity.LoginSucesses);
                ServerMessages.AddRange(accountActivity.LoginFails);
            }
        }
    }
}
