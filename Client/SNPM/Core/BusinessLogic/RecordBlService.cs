using Newtonsoft.Json;
using SNPM.Core.BusinessLogic.Interfaces;
using SNPM.Core.Interfaces;
using SNPM.Core.Interfaces.Api;
using SNPM.MVVM.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SNPM.Core.BusinessLogic
{
    internal class RecordBlService : IRecordBlService
    {
        private readonly IApiService apiService;
        private readonly IAccountBlService accountBlService;
        private readonly IDirectoryBlService directoryBlService;

        public RecordBlService(
            IApiService apiService,
            IAccountBlService accountBlService,
            IDirectoryBlService directoryBlService)
        {
            this.apiService = apiService;
            this.accountBlService = accountBlService;
            this.directoryBlService = directoryBlService;
        }

        public async Task<IEnumerable<IRecord>> GetRecordsFromDirectory(int directoryId)
        {
            if (accountBlService.ActiveToken == null)
            {
                throw new Exception("Not authenthicated");
            }

            var (success, serializedJson) = await apiService.GetRecordsFromDirectory(directoryId, accountBlService.ActiveToken.SessionToken);

            switch (success)
            {
                case "OK":
                    break;
                default:
                    throw new Exception(success);
            }

            var records = DeserializeJsonIntoObject<IEnumerable<Record>>(serializedJson);
            foreach (var record in records)
            {
                record.DirectoryName = directoryBlService.GetCachedDirectoryName(record.DirectoryId);
            }

            return records;
        }

        private T DeserializeJsonIntoObject<T>(string serializedJson)
        {
            var result = JsonConvert.DeserializeObject<T>(serializedJson) ?? throw new Exception("Deserialization failed");

            return result;
        }
    }
}
