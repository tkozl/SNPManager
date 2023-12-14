using SNPM.Core.BusinessLogic;
using SNPM.MVVM.Models.Interfaces;
using System;

namespace SNPM.MVVM.Models
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
                                                        | PasswordQuality.DictionaryFailed;

        public CharacterGroup RequiredCharacterGroups { get; set; }

        public static CharacterGroup AllCharacterGroups => CharacterGroup.Lowercase
                                                           | CharacterGroup.Uppercase
                                                           | CharacterGroup.Numeric
                                                           | CharacterGroup.Special;
    }
}
