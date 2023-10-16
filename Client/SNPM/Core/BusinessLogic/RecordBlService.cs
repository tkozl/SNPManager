using Newtonsoft.Json;
using SNPM.Core.BusinessLogic.Interfaces;
using SNPM.MVVM.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using SNPM.Core.Events;
using SNPM.Core.Api.Interfaces;
using SNPM.MVVM.Models.Interfaces;
using System.Text.RegularExpressions;

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
                record.Lifetime = record.LastUpdated.AddDays(record.DayLifetime);
            }

            return records;
        }

        public async Task<IRecord> GetRecord(int recordId)
        {
            if (accountBlService.ActiveToken == null)
            {
                throw new Exception("Not authenthicated");
            }

            var (success, serializedJson) = await apiService.GetRecord(recordId, accountBlService.ActiveToken.SessionToken);

            switch (success)
            {
                case "OK":
                    break;
                default:
                    throw new Exception(success);
            }

            return DeserializeJsonIntoObject<Record>(serializedJson);
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
            var newLifetime = (verifiedRecord.Lifetime - DateTime.UtcNow).Days + 1;

            if (!verifiedRecord.Errors.Any())
            {
                var (success, serializedJson) = await apiService.CreateRecord(createdRecord, accountBlService.ActiveToken.SessionToken, path, newLifetime);

                switch (success)
                {
                    case "Created":
                        break;
                    case "NoContent":
                        var cachedRecord = cachedRecords.First(x => x.EntryId == verifiedRecord.EntryId);
                        cachedRecord.CloneProperties(verifiedRecord);
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

        public async Task<IEnumerable<string>> GetCompatibleRecordPasswords(string targetString)
        {
            var records = GetComptaibleRecords(targetString);

            var passwords = new List<string>();

            // We take no more than 10 records to avoid DoS-ing the server
            foreach (var record in records.Take(10))
            {
                var fullRecord = await GetRecord(record.EntryId);

                passwords.Add(fullRecord.Password);
            }

            return passwords;
        }

        public async Task DeleteRecord(int recordId)
        {
            var trashId = directoryBlService.GetTrashDirectoryId();
            var record = cachedRecords.First(x => x.EntryId == recordId);
            var deletedName = record.Name + $"_D_{DateTime.UtcNow.ToFileTimeUtc()}";

            await MoveRecord(recordId, trashId, deletedName);
        }

        public async Task MoveRecord(int recordId, int targetDirectoryId, string nameOverride = "")
        {
            if (accountBlService.ActiveToken == null)
            {
                throw new Exception("Not authenthicated");
            }

            var record = cachedRecords.First(x => x.EntryId == recordId);

            var body = new
            {
                targetDirectoryID = targetDirectoryId,
                entryName = nameOverride ?? record.Name
            };

            var (success, _) = await apiService.CreateRecord(body, accountBlService.ActiveToken.SessionToken, recordId.ToString());

            switch (success)
            {
                case "OK":
                    break;
                default:
                    break;
            }
        }

        private IEnumerable<IRecord> GetComptaibleRecords(string targetString)
        {
            var matchedRecords = new List<IRecord>();

            foreach (var record in cachedRecords)
            {
                if (record.RelatedWindows.Any(x => Regex.IsMatch(targetString, x)))
                {
                    matchedRecords.Add(record);
                }
            }

            return matchedRecords;
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
