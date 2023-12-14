using System.Collections.Generic;

namespace SNPM.MVVM.Models.Interfaces
{
    public interface IErrorContainer
    {
        ICollection<KeyValuePair<string, string>> Errors { get; }

        void AddError(string propertyName, string errorMessage);

        void ClearErrors();
    }
}
