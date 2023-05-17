using SNPM.MVVM.Models;
using System;

namespace SNPM.Core.Interfaces
{
    [Flags]
    public enum PasswordQuality
    {
        None = 0,
        InvalidLength,
        NotEnoughWordGroups, // Uppercase, lowercase etc
        DictionaryFailed, // Checked remotely
        ContainsCommonWord // Password contains username, service name etc
    }

    internal interface IPasswordPolicy
    {

        public int Length { get; }

        public PasswordQuality PasswordQuality { get; }

        public CharacterGroup RequiredCharacterGroups { get; }

        public bool ShouldBeRemotelyVerified { get; }
    }
}
