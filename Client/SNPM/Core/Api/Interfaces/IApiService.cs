﻿using SNPM.MVVM.Models.Interfaces;
using System;
using System.Threading.Tasks;

namespace SNPM.Core.Api.Interfaces
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
        Task<string> CreateAccount(string mail, string password, EncryptionType encryptionType);

        Task<(string, string)> Login(string mail, string password);

        Task<(string, string)> AuthorizeSecondFactor(string code, string sessionToken);

        Task<(string, string)> RefreshToken(string sessionToken);

        Task<(string, string)> CreateDirectory(int parentId, string name, string sessionToken);

        Task<(string, string)> GetDirectories(int directoryId, string sessionToken);

        Task<(string, string)> MoveDirectory(int directoryId, string newName, int parentId, string sessionToken);

        Task<(string, string)> GetDirectoryData(int directoryId, string sessionToken);

        Task<(string, string)> GetSpecialDirectories(string sessionToken);

        Task<(string, string)> GetRecordsFromDirectory(int directoryId, string sessionToken);

        Task<(string, string)> CreateRecord(IRecord createdRecord, string sessionToken, string id, int newLifetime);

        Task<(string, string)> CreateRecord(dynamic body, string sessionToken, string id);

        Task<(string, string)> GetRecord(int recordId, string sessionToken);

        Task<(string, string)> GetAccountActivity(string sessionToken);

        Task<(string, string)> Disable2Fa(string sessionToken);

        Task<(string, string)> Enable2Fa(string sessionToken);

        Task<(string, string)> CheckPasswordStrength(string password);
    }
}
