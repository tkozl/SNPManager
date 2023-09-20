using SNPM.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNPM.Core.BusinessLogic.Interfaces
{
    public interface IRecordBlService
    {
        Task<IEnumerable<IRecord>> GetRecordsFromDirectory(int directoryId);
    }
}
