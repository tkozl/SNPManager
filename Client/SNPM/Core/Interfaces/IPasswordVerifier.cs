using System;
using System.Threading.Tasks;

namespace SNPM.Core.Interfaces
{
    [Flags]
    public enum PasswordQuality
    {
        None = 0,
        InvalidLength, 
        NotEnoughWordGroups, // Uppercase, lowercase etc
        DictionaryFailed, // Checked remotely
        ContainsCommonWord // Password contains username, service name etc
    }
    internal interface IPasswordVerifier
    {
        public Task<PasswordQuality> VerifyPassword(string password);

    }
}
