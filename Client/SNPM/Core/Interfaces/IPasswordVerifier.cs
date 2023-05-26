using System;
using System.Threading.Tasks;

namespace SNPM.Core.Interfaces
{
    public interface IPasswordVerifier
    {
        public Task<PasswordQuality> VerifyPassword(string password);

        public IPasswordPolicy PasswordPolicy { get; set; }
    }
}
