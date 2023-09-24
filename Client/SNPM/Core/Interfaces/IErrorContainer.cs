using System.Collections.Generic;

namespace SNPM.Core.Interfaces
{
    public interface IErrorContainer
    {
        ICollection<KeyValuePair<string, string>> Errors { get; }

        void AddError(string propertyName, string errorMessage);

        void ClearErrors();
    }
}
