using SNPM.MVVM.Models.Interfaces;
using System.Collections.Generic;

namespace SNPM.MVVM.Models
{
    internal class ErrorContainer : IErrorContainer
    {
        public ErrorContainer()
        {
            Errors = new List<KeyValuePair<string, string>>();
        }

        public ICollection<KeyValuePair<string, string>> Errors { get; }

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
