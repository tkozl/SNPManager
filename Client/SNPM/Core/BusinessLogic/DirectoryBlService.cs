using Newtonsoft.Json;
using SNPM.Core.BusinessLogic.Interfaces;
using SNPM.Core.Interfaces.Api;
using SNPM.MVVM.Models.UiModels;
using SNPM.MVVM.Models.UiModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SNPM.Core.BusinessLogic
{
    public class DirectoryBlService : IDirectoryBlService
    {
        private readonly IApiService apiService;
        private readonly IAccountBlService accountBlService;

        public DirectoryBlService(IApiService apiService, IAccountBlService accountBlService)
        {
            this.apiService = apiService;
            this.accountBlService = accountBlService;
        }

        public async Task<IEnumerable<IDirectory>> GetDirectories(int directoryId)
        {
            if (accountBlService.ActiveToken == null)
            {
                throw new Exception("Not authenthicated");
            }

            var (success, serializedJson) = await apiService.GetDirectories(directoryId, accountBlService.ActiveToken.SessionToken);

            switch (success)
            {
                case "OK":
                    break;
                default:
                    throw new Exception(success);
            }

            var directories = DeserializeJsonIntoDirectories(serializedJson);

            return directories;
        }

        public async Task<int> CreateDirectory(int parentId, string name)
        {
            if (accountBlService.ActiveToken == null)
            {
                throw new Exception("Not authenthicated");
            }

            var (success, serializedJson) = await apiService.CreateDirectory(parentId, name, accountBlService.ActiveToken.SessionToken);

            switch (success)
            {
                case "NoContent":
                case "Created":
                    break;
                default:
                    throw new Exception(success);
            }

            return DeserializeJsonIntoId(serializedJson);
        }

        public async Task MoveDirectory(int directoryId, string newName, int parentId)
        {
            if (accountBlService.ActiveToken == null)
            {
                throw new Exception("Not authenthicated");
            }

            var (success, serializedJson) = await apiService.MoveDirectory(directoryId, newName, parentId, accountBlService.ActiveToken.SessionToken);

            switch (success)
            {
                case "NoContent":
                    break;
                default:
                    throw new Exception(success);
            }
        }

        private IEnumerable<IDirectory> DeserializeJsonIntoDirectories(string serializedJson)
        {
            var result = JsonConvert.DeserializeObject<List<Directory>>(serializedJson) ?? throw new Exception("Directory deserialization failed");

            return result;
        }

        private int DeserializeJsonIntoId(string serializedJson)
        {
            var result = JsonConvert.DeserializeObject<Dictionary<string, int>>(serializedJson) ?? throw new Exception("Directory deserialization failed");
            result.TryGetValue("id", out var id);
            
            return id;
        }
    }
}
