using System;
using System.Threading.Tasks;

namespace SNPM.Core.Interfaces.Api
{
    public enum EncryptionType
    {
        Aes256 = 0
    }

    public enum HttpMethod
    {
        Get = 0,
        Post = 1,
        Delete = 2,
        Put = 3,
    }

    public interface IApiService
    {
        Func<string, Task<bool>> GetRemoteVerifier();

        Task<string> CreateAccount(string mail, string password, EncryptionType encryptionType);

        Task<bool> ModifyAccount(string currentPassword, string? newMail, string? newPassword);

        Task<IAccount> GetAccountInfo(int correctCount, int incorrectCount);

        Task<bool> VerifyEmail();

        Task<(string, string)> Login(string mail, string password);

        Task<(string, string)> CreateDirectory(int parentId, string name, string sessionToken);

        Task<(string, string)> GetDirectories(int directoryId, string sessionToken);

        Task<(string, string)> MoveDirectory(int directoryId, string newName, int parentId, string sessionToken);

        Task<(string, string)> GetDirectoryData(int directoryId, string sessionToken);

        Task<(string, string)> GetSpecialDirectories(string sessionToken);

        Task<(string, string)> GetRecordsFromDirectory(int directoryId, string sessionToken);
    }
}
