using SNPM.Core;
using SNPM.Core.BusinessLogic.Interfaces;
using SNPM.Core.Extensions;
using SNPM.MVVM.Models.Interfaces;
using SNPM.MVVM.Models.UiModels;
using SNPM.MVVM.Models.UiModels.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SNPM.MVVM.Models
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

            uiAccount.Is2FaRequired = domainAccount.Errors.ContainsKey(AccountError.RequiresSecondFactor);

            HandleErrors(uiAccount, domainAccount);
        }

        public async Task<bool> AuthorizeSecondFactor(string code)
        {
            return await accountBlService.AuthorizeSecondFactor(code);
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

        public async Task<IEnumerable<IUiDirectory>> GetDirectories(int directoryId, bool forceRefresh = false)
        {
            var domainDirectories = await directoryBlService.GetDirectories(directoryId, forceRefresh);

            var uiDirectories = new List<IUiDirectory>();
            foreach (var directory in domainDirectories)
            {
                uiDirectories.Add(new UiDirectory(directory.Id, directory.ParentId, directory.Name));
            }

            return uiDirectories;
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

        public async Task<IEnumerable<IUiRecord>> GetDirectoryRecords(int directoryId)
        {
            var domainRecords = await recordBlService.GetRecordsFromDirectory(directoryId);
            return domainRecords.Select(x => new UiRecord(x));
        }

        public async Task<IUiRecord> CreateRecord(IUiRecord createdRecord, int? currentId)
        {
            var domainRecord = new Record(createdRecord)
            {
                EntryId = currentId ?? 0
            };
            var res = await recordBlService.CreateRecord(domainRecord, currentId);
            var uiRecord = new UiRecord(res);

            return uiRecord;
        }

        public async Task DeleteRecord(IUiRecord uiRecord)
        {
            await recordBlService.DeleteRecord(uiRecord.EntryId);
        }

        public IAccountActivity GetAccountActivity()
        {
            return accountBlService.AccountActivity;
        }

        public async Task<string> Toggle2Fa()
        {
            var res = await accountBlService.Toggle2Fa();

            return res;
        }

        private static void HandleErrors(IUiAccount uiAccount, IAccount domainAccount)
        {
            uiAccount.Errors.Clear();
            foreach (var error in domainAccount.Errors)
            {
                // TODO: Replace ugly "ERROR: " with an icon
                if (error.Key != AccountError.RequiresSecondFactor)
                {
                    uiAccount.Errors.Add($"ERROR: {error.Key.GetDescription()}");
                }
            }
        }
    }
}
