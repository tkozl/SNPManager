using System;

namespace SNPM.Core.Interfaces
{
    public interface IRecord
    {
        string Comment { get; set; }
        string Location { get; set; }
        string Name { get; set; }
        string Username { get; set; }
        DateTime LastAccess { get; set; }
    }
}