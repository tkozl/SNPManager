using Newtonsoft.Json;
using SNPM.MVVM.Models.UiModels.Interfaces;
using System.Windows.Controls;

namespace SNPM.MVVM.Models.UiModels
{
    class Directory : IDirectory
    {
        public Directory(string name, int id, int parentId)
        {
            Name = name;
            Id = id;
            ParentId = parentId;
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("parentID")]
        public int ParentId { get; set; }
    }
}
