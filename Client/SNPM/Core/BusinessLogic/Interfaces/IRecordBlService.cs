using SNPM.MVVM.Models;
using SNPM.MVVM.Models.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SNPM.Core.BusinessLogic.Interfaces
{
    public interface IRecordBlService
    {
        Task<IEnumerable<IRecord>> GetRecordsFromDirectory(int directoryId);

        Task<IRecord> CreateRecord(IRecord createdRecord, int? id);
    }
}
