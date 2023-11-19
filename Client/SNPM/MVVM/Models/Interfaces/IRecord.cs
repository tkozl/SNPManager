using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SNPM.MVVM.Models.Interfaces
{
    public interface IRecord : IErrorContainer
    {
        int EntryId { get; set; }

        int DirectoryId { get; }

        string DirectoryName { get; set; }

        string Name { get; set; }

        string Username { get; set; }

        string Password { get; set; }

        ICollection<string> RelatedWindows { get; set; }

        ICollection<Parameter> Parameters { get; set; }

        string Note { get; set; }

        DateTime Lifetime { get; }

        DateTime LastUpdated { get; }

        int DayLifetime { get; set; }

        void CloneProperties(IRecord record);
    }
}