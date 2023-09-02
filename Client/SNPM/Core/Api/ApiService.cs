using SNPM.Core.Interfaces;
using SNPM.Core.Interfaces.Api;
using SNPM.MVVM.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Net;

namespace SNPM.Core.Api
{
    internal class ApiService : IApiService
    {
        private readonly HttpClient httpClient;
        private readonly string serverString;

        private static readonly Dictionary<EncryptionType, string> EncryptionTranslators = new()
        {
            {EncryptionType.Aes256, "aes-256" }
        };

        public ApiService()
        {
            // TODO: THIS IS A BYPASS, NEED MORE PERMAMENT SOLUTION
            // Related exception: (AuthenticationException: The remote certificate is invalid because of errors in the certificate chain: UntrustedRoot)
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            httpClient = new(clientHandler)
            {
                Timeout = TimeSpan.FromSeconds(10)
            };

            this.serverString = @"https://83.18.180.22:2137/api/v1";


            //httpClient.BaseAddress = new Uri(@"https://83.18.180.22:2137/api/v1");
        }

        public async Task<string> CreateAccount(string mail, string password, EncryptionType encryptionType)
        {
            var body = new
            {
                mail = mail,
                password = password,
                encryptionType = EncryptionTranslators[encryptionType]
            };

            var result = await RequestAsync("/account", body);

            return result.Item2;
        }

        public Task<IAccount> GetAccountInfo(int correctCount, int incorrectCount)
        {
            throw new NotImplementedException();
        }

        public Func<string, Task<bool>> GetRemoteVerifier()
        {
            return async _ => true;
        }

        public async Task<(string, string)> Login(string mail, string password)
        {
            var body = new
            {
                mail = mail,
                password = password
            };

            return await RequestAsync("/login", body);
        }

        public Task<bool> ModifyAccount(string currentPassword, string? newMail, string? newPassword)
        {
            throw new NotImplementedException();
        }

        public Task<bool> VerifyEmail()
        {
            throw new NotImplementedException();
        }

        private async Task<(string, string)> RequestAsync(string route, object body)
        {
            var jsonBody = JsonConvert.SerializeObject(body);
            var requestContent = new StringContent(jsonBody, System.Text.Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync($"{serverString}{route}", requestContent);
            string responseBody = await response.Content.ReadAsStringAsync();

            return (response.StatusCode!.ToString(), responseBody);
        }
    }
}
