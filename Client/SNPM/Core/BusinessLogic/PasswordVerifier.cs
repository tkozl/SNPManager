using SNPM.Core;
using SNPM.Core.Api.Interfaces;
using SNPM.Core.BusinessLogic.Interfaces;
using SNPM.Core.Extensions;
using SNPM.Core.Helpers.Interfaces;
using SNPM.MVVM.Models;
using SNPM.MVVM.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace SNPM.Core.BusinessLogic
{
    [Flags]
    public enum CharacterGroup
    {
        None = 0,
        Lowercase = 1,
        Uppercase = 2,
        Numeric = 4,
        Special = 8
    }

    public enum HashType
    {
        MD5,
        SHA256
    }

    public class PasswordVerifierService : IPasswordVerifierService
    {
        private IApiService apiService;
        private readonly IJsonHelper jsonHelper;
        public static HashType HashType;

        private static readonly CharacterGroup DefaultGroups =
            CharacterGroup.Lowercase |
            CharacterGroup.Uppercase |
            CharacterGroup.Numeric |
            CharacterGroup.Special;

        private HashType RemoteHashType;

        public IPasswordPolicy PasswordPolicy { get; set; }

        private static Dictionary<CharacterGroup, Regex> RegexTests = new Dictionary<CharacterGroup, Regex> {
           {CharacterGroup.Lowercase, new Regex(@"[a-z]") },
           {CharacterGroup.Uppercase, new Regex(@"[A-Z]") },
           {CharacterGroup.Numeric, new Regex(@"[0-9]") },
           {CharacterGroup.Special, new Regex(@"[^a-zA-Z0-9]") } 
        };

        public PasswordVerifierService(IApiService apiService, IJsonHelper jsonHelper)
        {
            this.apiService = apiService;
            this.jsonHelper = jsonHelper;
            RemoteHashType = HashType;

            PasswordPolicy = new PasswordPolicy(10, true);
        }

        public async Task<PasswordQuality> VerifyPassword(string password)
        {
            bool dictionaryVerification;
            if (PasswordPolicy.ShouldBeRemotelyVerified)
            {
                var dictionaryVerificationTask = VerifyDictionary(password);
                dictionaryVerification = await dictionaryVerificationTask;
            }
            else
            {
                dictionaryVerification = true;
            }

            var lengthVerification = VerifyLength(password);
            var groupVerification = VerifyWordGroups(password);

            return (lengthVerification ? PasswordQuality.None : PasswordQuality.InvalidLength) |
                   (groupVerification ? PasswordQuality.None : PasswordQuality.NotEnoughWordGroups) | // TODO: remote
                   (dictionaryVerification ? PasswordQuality.None : PasswordQuality.DictionaryFailed);
        }

        private bool VerifyLength(string password)
        {
            return password.Length >= PasswordPolicy.Length;
        }

        private bool VerifyWordGroups(string password)
        {
            foreach (CharacterGroup flag in PasswordPolicy.RequiredCharacterGroups.GetUniqueFlags())
            {
                if (!RegexTests[flag].IsMatch(password))
                {
                    return false;
                }
            }

            return true;
        }

        private async Task<bool> VerifyDictionary(string password)
        {
            var (succes, serializedJson) = await apiService.CheckPasswordStrength(password);

            switch (succes)
            {
                case "OK":
                    // Login succesful
                    break;
            }

            return (bool)jsonHelper.DeserializeJsonIntoDictionary(serializedJson)["strong"];
        }
    }
}
