using SNPM.Core.Interfaces;
using SNPM.Core.Interfaces.Api;
using SNPM.MVVM.Models;
using System;
using System.Threading.Tasks;

namespace SNPM.Core.Api
{
    public class PlaceholderApiService/* : IApiService*/
    {
        public PlaceholderApiService()
        {

        }

        public Task<bool> CreateAccount(string mail, string password, HashType encryptionType)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CreateAccount(string mail, string password, EncryptionType encryptionType)
        {
            throw new NotImplementedException();
        }

        public Task<IAccount> GetAccountInfo(int correctCount, int incorrectCount)
        {
            throw new NotImplementedException();
        }

        public Func<string, Task<bool>> GetRemoteVerifier()
        {
            return VerifyPasswordRemotely;
        }

        public Task<bool> Login(string mail, string password)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ModifyAccount(string currentPassword, string? newMail, string? newPassword)
        {
            throw new NotImplementedException();
        }

        public Task<bool> VerifyEmail()
        {
            throw new NotImplementedException();
        }

        private async Task<bool> VerifyPasswordRemotely(string passwordHash)
        {
            await Task.Delay(1);
            return true;
        }
    }
}
