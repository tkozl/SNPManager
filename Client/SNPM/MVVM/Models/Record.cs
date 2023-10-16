using Newtonsoft.Json;
using SNPM.Core;
using SNPM.Core.Converters;
using SNPM.MVVM.Models.Interfaces;
using SNPM.MVVM.Models.UiModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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

        public DateTime Lifetime { get; set; }

        [JsonProperty("passwordUpdateTime")]
        [JsonConverter(typeof(UtcDateTimeConverter))]
        public DateTime LastUpdated { get; set; }

        [JsonProperty("lifetime")]
        public int DayLifetime{ get; set; }

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
            this.Lifetime = uiRecord.Lifetime;
            this.Note = uiRecord.Note;
            this.RelatedWindows = uiRecord.RelatedWindows.Where(x => x.WindowName != string.Empty).Select(x => x.WindowName).ToList();

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

        public void CloneProperties(IRecord record)
        {
            this.EntryId = record.EntryId;
            this.DirectoryId = record.DirectoryId;
            this.DirectoryName = record.DirectoryName;
            this.Name = record.Name;
            this.Username = record.Username;
            this.Password = record.Password;
            this.Lifetime = record.Lifetime;
            this.RelatedWindows = new ObservableCollection<string>(record.RelatedWindows);
            this.Note = record.Note;
        }
    }
}
