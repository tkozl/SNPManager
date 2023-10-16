using SNPM.MVVM.Models.UiModels.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SNPM.MVVM.Models.Interfaces
{
    public interface IProxyService
    {
        Task CreateAccount(IUiAccount account);

        Task Login(IUiAccount uiAccount);

        Task<IEnumerable<IUiDirectory>> GetDirectories(int directoryId, bool forceRefresh);

        Task<int> CreateDirectory(int directoryId, string name);

        Task MoveDirectory(int directoryId, string newName, int parentId);

        Task DeleteDirectory(int id);

        Task<IEnumerable<IUiRecord>> GetDirectoryRecords(int directoryId);

        Task<IUiRecord> CreateRecord(IUiRecord createdRecord, int? currentId);

        Task DeleteRecord(IUiRecord uiRecord);
    }
}