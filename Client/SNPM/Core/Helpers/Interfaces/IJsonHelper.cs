using SNPM.Core.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNPM.Core.Helpers.Interfaces
{
    public interface IJsonHelper
    {
        IDictionary<string, object> DeserializeJsonIntoDictionary(string serializedJson);

        ICollection<Error> DeserializeJsonIntoErrors(string serializedJson);

        T DeserializeJsonIntoObject<T>(string serializedJson);
    }
}
