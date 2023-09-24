using Newtonsoft.Json;
using SNPM.Core;
using SNPM.Core.Converters;
using SNPM.Core.Interfaces;
using SNPM.MVVM.Models.UiModels.Interfaces;
using System;
using System.Collections.Generic;

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
        public ICollection<string> RelatedWindows { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }

        public DateTime Lifetime => LastUpdated.AddDays(DayLifetime);

        [JsonProperty("passwordUpdateTime")]
        [JsonConverter(typeof(UtcDateTimeConverter))]
        public DateTime LastUpdated { get; set; }

        [JsonProperty("lifetime")]
        public int DayLifetime { get; set; }

        public ICollection<KeyValuePair<string, string>> Errors { get; }

        public Record()
        {
            Errors = new List<KeyValuePair<string, string>>();
        }

        public Record(IUiRecord uiRecord, int entryId) : this(uiRecord)
        {
            this.EntryId = entryId;
        }

        public Record(IUiRecord uiRecord)
        {
            this.DirectoryId = uiRecord.DirectoryId;
            this.Name = uiRecord.Name;
            this.Username = uiRecord.Username;
            this.Password = uiRecord.Password;
            this.Note = uiRecord.Note;
            this.RelatedWindows = uiRecord.RelatedWindows;

            Errors = new List<KeyValuePair<string, string>>();
        }

        public void AddError(string propertyName, string errorMessage)
        {
            Errors.Add(new KeyValuePair<string, string>(propertyName, errorMessage));
        }

        public void ClearErrors()
        {
            Errors.Clear();
        }
    }
}
