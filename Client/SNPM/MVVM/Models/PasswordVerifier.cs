using SNPM.Core;
using SNPM.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace SNPM.MVVM.Models
{
    [Flags]
    public enum CharacterGroup
    {
        None,
        Lowercase,
        Uppercase,
        Numeric,
        Special
    }

    public enum HashType
    {
        MD5,
        SHA256
    }

    internal class PasswordVerifier : IPasswordVerifier
    {
        //public delegate bool RemoteDictionaryVerifier(string passwordHash);
        
        private static readonly CharacterGroup DefaultGroups = 
            CharacterGroup.Lowercase | 
            CharacterGroup.Uppercase | 
            CharacterGroup.Numeric | 
            CharacterGroup.Special;

        private Func<string, Task<bool>> RemoteVerifier;
        private HashType RemoteHashType;
        private IPasswordPolicy PasswordPolicy;
        

        public PasswordVerifier(Func<string, Task<bool>> verifier, HashType hashType)
        {
            RemoteVerifier = verifier;
            RemoteHashType = hashType;

            PasswordPolicy = new PasswordPolicy(10, true);
        }

        public async Task<PasswordQuality> VerifyPassword(string password)
        {
            var dictionaryVerificationTask = VerifyDictionary(password);

            var lengthVerification = VerifyLength(password);
            var groupVerification = VerifyWordGroups(password);

            var dictionaryVerification = await dictionaryVerificationTask;

            return (lengthVerification ? PasswordQuality.None : PasswordQuality.InvalidLength) |
                   (groupVerification ? PasswordQuality.None : PasswordQuality.NotEnoughWordGroups) |
                   (dictionaryVerification ? PasswordQuality.None : PasswordQuality.DictionaryFailed);
        }

        private bool VerifyLength(string password)
        {
            return password.Length >= PasswordPolicy.Length;
        }

        private bool VerifyWordGroups(string password)
        {
            //if (PasswordPolicy.PasswordQuality.HasFlag())

            return false;
        }

        private async Task<bool> VerifyDictionary(string password)
        {
            // TODO: (Przemek) Implement after server has dictionary verification implemented.
            await Task.Delay(1);

            return false;
        }
    }
}
