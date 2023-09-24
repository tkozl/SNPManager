using Newtonsoft.Json;
using SNPM.Core.BusinessLogic.Interfaces;
using SNPM.Core.Interfaces;
using SNPM.Core.Interfaces.Api;
using SNPM.MVVM.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using SNPM.Core.Events;

namespace SNPM.Core.BusinessLogic
{
    internal class RecordBlService : IRecordBlService
    {
        private readonly IApiService apiService;
        private readonly IAccountBlService accountBlService;
        private readonly IDirectoryBlService directoryBlService;

        private ICollection<IRecord> cachedRecords;

        public RecordBlService(
            IApiService apiService,
            IAccountBlService accountBlService,
            IDirectoryBlService directoryBlService)
        {
            this.apiService = apiService;
            this.accountBlService = accountBlService;
            this.directoryBlService = directoryBlService;

            cachedRecords = new List<IRecord>();
            this.directoryBlService.DirectoriesLoaded += OnDirectoriesLoaded;
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

        public async Task<IRecord> CreateRecord(IRecord createdRecord, int? id)
        {
            if (accountBlService.ActiveToken == null)
            {
                throw new Exception("Not authenthicated");
            }

            var verifiedRecord = ClientVerifyRecord(createdRecord, id);
            verifiedRecord.EntryId = id ?? 0;

            string path = id?.ToString() ?? string.Empty;

            if (!verifiedRecord.Errors.Any())
            {
                var (success, serializedJson) = await apiService.CreateRecord(createdRecord, accountBlService.ActiveToken.SessionToken, path);

                switch (success)
                {
                    case "Created":
                        break;
                    case "NoContent":
                        return verifiedRecord;
                    default:
                        throw new Exception(success);
                }

                DeserializeJsonIntoObject<Dictionary<string, int>>(serializedJson).TryGetValue("id", out var recordId);

                verifiedRecord.EntryId = recordId;
                verifiedRecord.DirectoryName = directoryBlService.GetCachedDirectoryName(verifiedRecord.DirectoryId);
            }

            return verifiedRecord;
        }

        private IRecord ClientVerifyRecord(IRecord recordToVerify, int? id)
        {
            // Name verification
            var nameConfilct = cachedRecords.Any(x => x.Name == recordToVerify.Name && x.EntryId != id);
            if (nameConfilct)
            {
                recordToVerify.AddError(nameof(recordToVerify.Name), "Duplicate name");
            }

            return recordToVerify;
        }

        private async void OnDirectoriesLoaded(object? sender, EventArgs e)
        {
            var allRecords = await GetRecordsFromDirectory(0);
            cachedRecords = new List<IRecord>(allRecords);
        }

        private T DeserializeJsonIntoObject<T>(string serializedJson)
        {
            var result = JsonConvert.DeserializeObject<T>(serializedJson) ?? throw new Exception("Deserialization failed");

            return result;
        }
    }
}
