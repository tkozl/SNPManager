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

        Task<IRecord> GetRecord(int recordId);

        Task DeleteRecord(int recordId);

        Task<IEnumerable<string>> GetCompatibleRecordPasswords(string targetString);

        Task MoveRecord(int recordId, int targetDirectoryId, string nameOverride = null);
    }
}
