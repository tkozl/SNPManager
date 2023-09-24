using SNPM.MVVM.Models.UiModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SNPM.Core.BusinessLogic.Interfaces
{
    public interface IDirectoryBlService
    {
        event EventHandler DirectoriesLoaded;

        Task<IEnumerable<IDirectory>> GetDirectories(int directoryId);

        Task<IDirectory> GetDirectory(int id, bool forceRefresh = true);

        Task<int> CreateDirectory(int parentId, string name);

        Task MoveDirectory(int directoryId, string newName, int parentId);

        Task DeleteDirectory(int id);

        string GetCachedDirectoryName(int id);
    }
}
