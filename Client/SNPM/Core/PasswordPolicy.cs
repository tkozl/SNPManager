using SNPM.Core.Interfaces;
using SNPM.MVVM.Models;
using System;

namespace SNPM.Core
{
    public class PasswordPolicy : IPasswordPolicy
    {
        public PasswordPolicy(int Length, bool ShouldBeRemotelyVerified) : this(Length, ShouldBeRemotelyVerified, MaximumQuality, AllCharacterGroups) { }

        public PasswordPolicy(int Length, bool ShouldBeRemotelyVerified, PasswordQuality PasswordQuality, CharacterGroup RequiredCharacterGroups)
        {
            this.Length = Length;
            this.ShouldBeRemotelyVerified = ShouldBeRemotelyVerified;
            this.PasswordQuality = PasswordQuality;
            this.RequiredCharacterGroups = RequiredCharacterGroups;
        }

        public int Length { get; set; }

        public PasswordQuality PasswordQuality { get; set; }

        public bool ShouldBeRemotelyVerified { get; set; }

        public static PasswordQuality MaximumQuality => PasswordQuality.InvalidLength
                                                        | PasswordQuality.NotEnoughWordGroups
                                                        | PasswordQuality.DictionaryFailed
                                                        | PasswordQuality.ContainsCommonWord;

        public CharacterGroup RequiredCharacterGroups { get; set; }

        public static CharacterGroup AllCharacterGroups => CharacterGroup.Lowercase
                                                           | CharacterGroup.Uppercase
                                                           | CharacterGroup.Numeric
                                                           | CharacterGroup.Special;
    }
}
