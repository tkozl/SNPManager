using SNPM.Core.BusinessLogic.Interfaces;
using SNPM.Core.Extensions;
using SNPM.Core.Interfaces;
using SNPM.Core.Interfaces.Api;
using SNPM.MVVM.Models.UiModels;
using SNPM.MVVM.Models.UiModels.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SNPM.Core
{
    public class ProxyService : IProxyService
    {
        private readonly IAccountBlService accountBlService;
        private readonly IDirectoryBlService directoryBlService;
        private readonly IRecordBlService recordBlService;

        public ProxyService(
            IAccountBlService accountBlService,
            IDirectoryBlService directoryBlService,
            IRecordBlService recordBlService)
        {
            this.accountBlService = accountBlService;
            this.directoryBlService = directoryBlService;
            this.recordBlService = recordBlService;
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

        public async Task<IEnumerable<IDirectory>> GetDirectories(int directoryId)
        {
            return await directoryBlService.GetDirectories(directoryId);
        }

        public async Task<int> CreateDirectory(int directoryId, string name)
        {
            return await directoryBlService.CreateDirectory(directoryId, name);
        }

        public async Task MoveDirectory(int directoryId, string newName, int parentId)
        {
            await directoryBlService.MoveDirectory(directoryId, newName, parentId);
        }

        public async Task DeleteDirectory(int directoryId)
        {
            await directoryBlService.DeleteDirectory(directoryId);
        }

        public async Task<IEnumerable<IRecord>> GetDirectoryRecords(int directoryId)
        {
            return await recordBlService.GetRecordsFromDirectory(directoryId);
        }
    }
}
