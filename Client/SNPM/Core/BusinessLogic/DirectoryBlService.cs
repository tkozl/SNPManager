using Newtonsoft.Json;
using SNPM.Core.Api.Interfaces;
using SNPM.Core.BusinessLogic.Interfaces;
using SNPM.MVVM.Models.UiModels;
using SNPM.MVVM.Models.UiModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SNPM.Core.BusinessLogic
{
    public class DirectoryBlService : IDirectoryBlService
    {
        public event EventHandler DirectoriesLoaded;

        private readonly IApiService apiService;
        private readonly IAccountBlService accountBlService;

        private Dictionary<string, int> specialDirectories;
        private IEnumerable<IDirectory> cachedDirectories;

        public DirectoryBlService(IApiService apiService, IAccountBlService accountBlService)
        {
            this.apiService = apiService;
            this.accountBlService = accountBlService;

            cachedDirectories = Enumerable.Empty<IDirectory>();
            this.accountBlService.AccountLoggedIn += OnLogin;
        }

        public async Task<IEnumerable<IDirectory>> GetDirectories(int directoryId, bool forceRefresh)
        {
            if (accountBlService.ActiveToken == null)
            {
                throw new Exception("Not authenthicated");
            }

            if (!forceRefresh && cachedDirectories.Any())
            {
                return cachedDirectories;
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
            specialDirectories = await GetSpecialDirectories();

            var sdirs = new List<IDirectory>();
            foreach (var sdir in specialDirectories)
            {
                sdirs.Add(new Directory(sdir.Key, sdir.Value, int.MinValue));
            }
            cachedDirectories = directories.Concat(sdirs);
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

        public async Task DeleteDirectory(int id)
        {
            if (accountBlService.ActiveToken == null)
            {
                throw new Exception("Not authenthicated");
            }

            var directory = await GetDirectory(id);
            var trashId = GetTrashDirectoryId();

            await MoveDirectory(id, directory.Name, trashId);
        }

        public async Task<IDirectory> GetDirectory(int id, bool forceRefresh = true)
        {
            if (!forceRefresh)
            {
                var cachedDirectory = cachedDirectories.FirstOrDefault(x => x.Id == id);

                if (cachedDirectory != null)
                {
                    return cachedDirectory;
                }
            }

            var (success, serializedJson) = await apiService.GetDirectoryData(id, accountBlService.ActiveToken.SessionToken);

            switch (success)
            {
                case "OK":
                    break;
                default:
                    throw new Exception(success);
            }

            var directory = DeserializeJsonIntoObject<Directory>(serializedJson);
            directory.Id = id;
            return directory;
        }

        public string GetCachedDirectoryName(int id)
        {
            var cachedDirectory = cachedDirectories.First(x => x.Id == id);

            return cachedDirectory.Name;
        }

        public int GetTrashDirectoryId()
        {
            specialDirectories.TryGetValue("trash", out var trashId);

            return trashId;
        }

        private async void OnLogin(object? sender, EventArgs e)
        {
            await GetDirectories(0, true);
            DirectoriesLoaded.Invoke(this, new EventArgs());
        }

        private async Task<Dictionary<string, int>> GetSpecialDirectories()
        {
            var (success, serializedJson) = await apiService.GetSpecialDirectories(accountBlService.ActiveToken.SessionToken);

            switch (success)
            {
                case "OK":
                    break;
                default:
                    throw new Exception(success);
            }

            return DeserializeJsonIntoObject<Dictionary<string, int>>(serializedJson);
        }

        private IEnumerable<IDirectory> DeserializeJsonIntoDirectories(string serializedJson)
        {
            var result = JsonConvert.DeserializeObject<List<Directory>>(serializedJson) ?? throw new Exception("Directory deserialization failed");

            return result;
        }

        private T DeserializeJsonIntoObject<T>(string serializedJson)
        {
            var result = JsonConvert.DeserializeObject<T>(serializedJson) ?? throw new Exception("Deserialization failed");

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
