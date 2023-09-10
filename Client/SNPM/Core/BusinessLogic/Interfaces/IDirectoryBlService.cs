using SNPM.MVVM.Models.UiModels.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SNPM.Core.BusinessLogic.Interfaces
{
    public interface IDirectoryBlService
    {
        Task<IEnumerable<IDirectory>> GetDirectories(int directoryId);

        Task MoveDirectory(int directoryId, string newName, int parentId);
    }
}
