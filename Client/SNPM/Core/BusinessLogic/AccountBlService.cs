using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using SNPM.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using SNPM.Core.BusinessLogic.Interfaces;
using SNPM.Core.Api.Interfaces;
using SNPM.MVVM.Models.Interfaces;
using SNPM.Core.Helpers.Interfaces;
using SNPM.MVVM.Models;
using System.Security;

namespace SNPM.Core.BusinessLogic
{
    public class Error
    {
        [JsonProperty("errorID")]
        public string ErrorId;

        [JsonProperty("description")]
        public string Description;

        public Error(string ErrorId, string Description)
        {
            this.ErrorId = ErrorId;
            this.Description = Description;
        }
    }

    internal class AccountBlService : IAccountBlService
    {
        private readonly IApiService apiService;
        private readonly IServiceProvider serviceProvider;
        private readonly IPasswordVerifierService passwordVerifier;
        
        private readonly IJsonHelper jsonHelper;
        private string Username;
        public event EventHandler AccountLoggedIn;

        public AccountBlService(
            IApiService apiService,
            IServiceProvider serviceProvider,
            IJsonHelper jsonHelper,
            IPasswordVerifierService passwordVerifier)
        {
            this.apiService = apiService;
            this.serviceProvider = serviceProvider;
            this.jsonHelper = jsonHelper;
            this.passwordVerifier = passwordVerifier;
        }
        public IAccountActivity AccountActivity { get; set; }

        public IToken? ActiveToken { get; set; }

        public async Task CreateAccount(IAccount account)
        {
            string serializedJson = await apiService.CreateAccount(account.Username, account.Password, account.Encryption);

            account.Errors.Clear();
            if (serializedJson.Length > 0)
            {
                var errors = jsonHelper.DeserializeJsonIntoErrors(serializedJson);

                foreach (var error in errors)
                {
                    //AccountError parsedError = Enum.Parse<AccountError>(error.ErrorId);
                    var parsedError = GetEnumFromEnumMemberValue(error.ErrorId);

                    account.Errors.Add(parsedError, error.Description);
                }
            }

        }

        public async Task Login(IAccount account)
        {
            var (succes, serializedJson) = await apiService.Login(account.Username, account.Password);

            switch (succes)
            {
                case "OK":
                    // Login succesful
                    break;
                default:
                    account.Errors.Add(AccountError.LoginFailed, EnumExtensions.GetDescription(AccountError.LoginFailed));
                    return;
            }

            if (serializedJson.Length > 0)
            {
                var result = jsonHelper.DeserializeJsonIntoDictionary(serializedJson);

                account.Errors.Clear();

                ActiveToken = serviceProvider.GetService<IToken>() ?? throw new Exception("Model not registered");
                ActiveToken.SessionToken = result["token"].ToString()!;
                ActiveToken.ExpirationTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(result["expiration"].ToString()!)).DateTime;
                ActiveToken.RefreshTokenMethod = RefreshToken;
                Username = account.Username;

                result.TryGetValue("is2faRequired", out var twoFa);

                if ((bool)twoFa!)
                {
                    account.Errors.Add(AccountError.RequiresSecondFactor, "Second authethincation required to login");
                }
                else
                {
                    AccountActivity = await GetAccountActivity(ActiveToken.SessionToken);

                    AccountLoggedIn.Invoke(this, new EventArgs());
                }
            }
        }

        public async Task<bool> AuthorizeSecondFactor(string code)
        {
            var (succes, serializedJson) = await apiService.AuthorizeSecondFactor(code, ActiveToken.SessionToken);
            switch (succes)
            {
                case "OK":
                    break;
                default:
                    return false;
            }

            var result = jsonHelper.DeserializeJsonIntoDictionary(serializedJson);

            ActiveToken = serviceProvider.GetService<IToken>() ?? throw new Exception("Model not registered");
            ActiveToken.SessionToken = result["token"].ToString()!;
            ActiveToken.ExpirationTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(result["expiration"].ToString()!)).DateTime;
            ActiveToken.RefreshTokenMethod = RefreshToken;

            AccountActivity = await GetAccountActivity(ActiveToken.SessionToken);

            AccountLoggedIn.Invoke(this, new EventArgs());
            return true;
        }

        public async Task<IAccountActivity> GetAccountActivity(string sessionToken)
        {
            var (succes, serializedJson) = await apiService.GetAccountActivity(sessionToken);

            switch (succes)
            {
                case "OK":
                    break;
                default:
                    throw new Exception("Something unexpected happened");
            }

            return jsonHelper.DeserializeJsonIntoObject<AccountActivity>(serializedJson);
        }

        public async Task<string> Toggle2Fa()
        {
            if (AccountActivity.Active2fa)
            {
                Disable2Fa();
                return string.Empty;
            }
            else
            {
                return await Enable2Fa();
            }
        }

        private async Task<string> Enable2Fa()
        {
            var (succes, serializedJson) = await apiService.Enable2Fa(ActiveToken.SessionToken);

            switch (succes)
            {
                case "Created":
                    break;
                default:
                    throw new Exception("Something unexpected happened");
            }

            var res = jsonHelper.DeserializeJsonIntoDictionary(serializedJson);
            res.TryGetValue("secretCode", out var token);
            var secretCode = token as string ?? throw new Exception("Deserialization failed");

            var uri = $"otpauth://totp/SNPM:{Username}?secret={secretCode}";

            return uri;
        }

        public async Task<PasswordQuality> VerifyPassword(string password)
        {
            return await passwordVerifier.VerifyPassword(password);
        }

        private async Task Disable2Fa()
        {
            var (succes, _) = await apiService.Disable2Fa(ActiveToken.SessionToken);

            switch (succes)
            {
                case "OK":
                    break;
                default:
                    throw new Exception("Something unexpected happened");
            }
        }

        private async Task<DateTime> RefreshToken(IToken token)
        {
            var (succes, serializedJson) = await apiService.RefreshToken(token.SessionToken);

            switch (succes)
            {
                case "OK":
                    break;
                case "Forbidden":
                    throw new Exception("Token can not be prolonged. Relaunch the application.");
                default:
                    throw new Exception("Something unexpected happened");
            }

            var res = jsonHelper.DeserializeJsonIntoDictionary(serializedJson);

            if (res.TryGetValue("token", out var newToken) && res.TryGetValue("expiration", out var expiration))
            {
                var expirationDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expiration.ToString()!)).DateTime;
                token.SessionToken = (string)newToken;

                return expirationDate;
            }
            else
            {
                return DateTime.UtcNow;
            }
        }

        static AccountError GetEnumFromEnumMemberValue(string input)
        {
            foreach (AccountError value in Enum.GetValues(typeof(AccountError)))
            {
                if (GetEnumMemberValue(value) == input)
                {
                    return value;
                }
            }

            throw new ArgumentException($"No matching enum value found for '{input}'.");
        }

        static string GetEnumMemberValue(AccountError value)
        {
            var memberInfo = typeof(AccountError).GetField(value.ToString());
            var enumMemberAttribute = (EnumMemberAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(EnumMemberAttribute));

            return enumMemberAttribute?.Value ?? value.ToString();
        }
    }
}
