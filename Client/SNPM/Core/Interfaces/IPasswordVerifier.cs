using System;
using System.Threading.Tasks;

namespace SNPM.Core.Interfaces
{
    internal interface IPasswordVerifier
    {
        public Task<PasswordQuality> VerifyPassword(string password);
    }
}
