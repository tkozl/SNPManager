using SNPM.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNPM.Core.Interfaces.Api
{
    public enum EncryptionType
    {
        Aes256 = 0
    }

    public interface IApiService
    {
        Func<string, Task<bool>> GetRemoteVerifier();

        Task<string> CreateAccount(string mail, string password, EncryptionType encryptionType);

        Task<bool> ModifyAccount(string currentPassword, string? newMail, string? newPassword);

        Task<IAccount> GetAccountInfo(int correctCount, int incorrectCount);

        Task<bool> VerifyEmail();

        Task<(string, string)> Login(string mail, string password);
    }
}
