using Newtonsoft.Json;
using SNPM.Core;
using SNPM.Core.Interfaces;
using System;

namespace SNPM.MVVM.Models
{
    public class Record : ObservableObject, IRecord
    {
        [JsonProperty("entryID")]
        public int EntryId { get; set; }

        [JsonProperty("directoryID")]
        public int DirectoryId { get; set; }

        public string DirectoryName { get; set; }

        [JsonProperty("entryName")]
        public string Name { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("relatedWindows")]
        public string RelatedWindows { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }

        public DateTime Lifetime { get; set; }

        [JsonProperty("lifetime")]
        public int DayLifetime
        {
            get => Lifetime.Subtract(DateTime.UtcNow).Days;
            set
            {
                Lifetime = DateTime.UtcNow.AddDays(value);
            }
        }
    }
}
