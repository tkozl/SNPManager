using Newtonsoft.Json;
using SNPM.Core.BusinessLogic;
using SNPM.Core.Helpers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNPM.Core.Helpers
{
    internal class JsonHelper : IJsonHelper
    {
        public IDictionary<string, object> DeserializeJsonIntoDictionary(string serializedJson)
        {
            return DeserializeJsonIntoObject<Dictionary<string, object>>(serializedJson);
        }

        public T DeserializeJsonIntoObject<T>(string serializedJson)
        {
            var result = JsonConvert.DeserializeObject<T>(serializedJson) ?? throw new Exception("Deserialization failed");

            return result;
        }

        public ICollection<Error> DeserializeJsonIntoErrors(string serializedJson)
        {
            ICollection<Error> result;

            var deserializationResult = JsonConvert.DeserializeObject<Dictionary<string, ICollection<Error>>>(serializedJson);
            if (deserializationResult is null)
            {
                throw new Exception("Deserialization failed");
            }
            else if (deserializationResult.Count != 1)
            {
                throw new Exception("Unexpected deserialization result");
            }
            else
            {
                deserializationResult.TryGetValue("errors", out result!);
            }

            return result;
        }
    }
}
