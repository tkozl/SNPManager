using SNPM.Core.BusinessLogic;
using SNPM.Core.Extensions;
using SNPM.Core.Interfaces;
using SNPM.Core.Interfaces.Api;
using SNPM.MVVM.Models.UiModels.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SNPM.Core
{
    public class ProxyService : IProxyService
    {
        private readonly IAccountBlService accountBlService;

        public ProxyService(IAccountBlService accountBlService)
        {
            this.accountBlService = accountBlService;
        }

        public async Task Login(IUiAccount uiAccount)
        {
            var domainAccount = new Account
            {
                Username = uiAccount.Username,
                Password = uiAccount.Password
            };

            await accountBlService.Login(domainAccount);

            HandleErrors(uiAccount, domainAccount);
        }

        public async Task CreateAccount(IUiAccount uiAccount)
        {
            var domainAccount = new Account
            {
                Username = uiAccount.Username,
                Password = uiAccount.Password
            };

            await accountBlService.CreateAccount(domainAccount);

            HandleErrors(uiAccount, domainAccount);
        }

        private void HandleErrors(IUiAccount uiAccount, IAccount domainAccount)
        {
            uiAccount.Errors.Clear();
            foreach (var error in domainAccount.Errors)
            {
                // TODO: Replace ugly "ERROR: " with an icon
                uiAccount.Errors.Add($"ERROR: {error.Key.GetDescription()}");
            }
        }
    }
}
