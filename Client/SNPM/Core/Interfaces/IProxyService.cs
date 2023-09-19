using SNPM.MVVM.Models.UiModels.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SNPM.Core.Interfaces
{
    public interface IProxyService
    {
        Task CreateAccount(IUiAccount account);

        Task Login(IUiAccount uiAccount);

        Task<IEnumerable<IDirectory>> GetDirectories(int directoryId);

        Task<int> CreateDirectory(int directoryId, string name);

        Task MoveDirectory(int directoryId, string newName, int parentId);

        Task DeleteDirectory(int id);
    }
}