using SNPM.Core.Interfaces.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace SNPM.Core.Interfaces
{
    public enum AccountError
    {
        [EnumMember(Value = "invalid_email"), Description("Email is not valid")]
        InvalidEmail,

        [EnumMember(Value = "too_low_password_complexity"), Description("Password complexity is too low")]
        TooLowPasswordComplexity,

        [EnumMember(Value = "too_long_string"), Description("Password is too long")]
        TooLongString,

        [EnumMember(Value = "unknown_encryption_type"), Description("Encryption type unknown")]
        UnknownEncryptionType,

        [EnumMember(Value = "is2faRequired"), Description("Second factor authenthication required to continue")]
        RequiresSecondFactor,

        [EnumMember(Value = "token_expired"), Description("Token has expired")]
        TokenExpired,

        [EnumMember(Value = "login_failed"), Description("Invalid username or password")]
        LoginFailed,

        // NotAuthenthicated,
    }

    public interface IAccount
    {
        string Username { get; set; }

        string Password { get; set; }

        IToken? AuthenthicationToken { get; set; }

        bool IsAuthenticated { get; }

        EncryptionType Encryption { get; }

        IDictionary<AccountError, string> Errors { get; }
    }
}
