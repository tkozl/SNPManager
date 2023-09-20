using Newtonsoft.Json;
using System;

namespace SNPM.Core.Interfaces
{
    public interface IRecord
    {
        int EntryId { get; }

        int DirectoryId { get; }

        string DirectoryName { get; set; }

        string Name { get; }

        string Username { get; }

        string Password { get; }

        string RelatedWindows { get; }

        string Note { get; }

        DateTime Lifetime { get; }

        int DayLifetime { get; set; }
    }
}