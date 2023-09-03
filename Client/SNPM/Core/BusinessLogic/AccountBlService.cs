using Newtonsoft.Json;
using SNPM.Core.Interfaces;
using SNPM.Core.Interfaces.Api;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using SNPM.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using SNPM.Core.BusinessLogic.Interfaces;

namespace SNPM.Core.BusinessLogic
{
    internal class Error
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
        public IToken? ActiveToken { get; set; }

        private readonly IApiService apiService;
        private readonly IServiceProvider serviceProvider;
        private static JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Include,
            MissingMemberHandling = MissingMemberHandling.Error,
        };
        public AccountBlService(IApiService apiService, IServiceProvider serviceProvider)
        {
            this.apiService = apiService;
            this.serviceProvider = serviceProvider;
        }

        public async Task CreateAccount(IAccount account)
        {
            string serializedJson = await apiService.CreateAccount(account.Username, account.Password, account.Encryption);

            account.Errors.Clear();
            if (serializedJson.Length > 0)
            {
                var errors = DeserializeJsonIntoErrors(serializedJson);

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
                var result = DeserializeJson(serializedJson);

                account.Errors.Clear();

                ActiveToken = serviceProvider.GetService<IToken>() ?? throw new Exception("Model not registered");
                ActiveToken.SessionToken = result["token"].ToString()!;
                ActiveToken.ExpirationTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(result["expiration"].ToString()!)).DateTime;
                ActiveToken.RefreshTokenMethod = RefreshToken;

                var twoFa = result["is2faRequired"].ToString() == "false";

                if (twoFa)
                {
                    account.Errors.Add(AccountError.RequiresSecondFactor, "Second authethincation required to login");
                }
            }
        }

        private DateTime RefreshToken(IToken token)
        {
            return DateTime.UtcNow + TimeSpan.FromMinutes(20);
        }

        // TODO: works but make it better
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

        private ICollection<Error> DeserializeJsonIntoErrors(string serializedJson)
        {
            ICollection<Error> result;
            //Dictionary<string, ICollection<Error>> result = new();
            try
            {
                var deserializationResult = JsonConvert.DeserializeObject<Dictionary<string, ICollection<Error>>>(serializedJson);
                if (deserializationResult is null)
                {
                    throw new Exception("Deserialization failed");
                }
                else if (deserializationResult.Count != 1)
                {
                    throw new Exception("Unexpected deserialization result");
                }
                else
                {
                    deserializationResult.TryGetValue("errors", out result!);
                }
            }
            catch (JsonException e)
            {
                // TODO: Handle unexpected server response

                throw new JsonException("Serialization failed - possible misunderstood server response");
            }

            return result;
        }

        private IDictionary<string, object> DeserializeJson(string serializedJson)
        {
            var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(serializedJson, JsonSerializerSettings);

            return result ?? new Dictionary<string, object>();
        }
    }
}
