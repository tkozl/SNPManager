using SNPM.Core.BusinessLogic;
using System;

namespace SNPM.MVVM.Models.Interfaces
{
    [Flags]
    public enum PasswordQuality
    {
        None = 0,
        InvalidLength = 1,
        NotEnoughWordGroups = 2, // Uppercase, lowercase etc
        DictionaryFailed = 4, // Checked remotely
        ContainsCommonWord = 8 // Password contains username, service name etc
    }

    public interface IPasswordPolicy
    {

        public int Length { get; }

        public PasswordQuality PasswordQuality { get; }

        public CharacterGroup RequiredCharacterGroups { get; }

        public bool ShouldBeRemotelyVerified { get; }
    }
}
