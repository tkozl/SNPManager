using SNPM.Core.Interfaces;
using SNPM.Core.Interfaces.Api;
using System;
using System.Collections.Generic;

namespace SNPM.Core
{
    public class Account : IAccount
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string SessionToken { get; set; } // TODO: (Przemek) Create a separate class for handling tokens
        public DateTime TokenExpirationDate { get; set; }
        public bool IsAuthenticated => SessionToken == string.Empty || DateTime.Now > TokenExpirationDate;

        public EncryptionType Encryption => EncryptionType.Aes256;

        public IDictionary<AccountError, string> Errors { get; }

        public Account() : this(string.Empty, string.Empty, string.Empty) { }
        private Account(string username, string password, string sessionToken)
        {
            Username = username;
            Password = password;

            SessionToken = sessionToken;
            if (sessionToken != string.Empty)
            {
                TokenExpirationDate = DateTime.Now.AddMinutes(20);
            }
            else
            {
                TokenExpirationDate = DateTime.MinValue;
            }

            Errors = new Dictionary<AccountError, string>
            {
            };
        }
    }
}
