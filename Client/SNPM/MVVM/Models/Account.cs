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
        
        public IToken? AuthenthicationToken { get; set; }
        public bool IsAuthenticated => AuthenthicationToken != null;

        public EncryptionType Encryption => EncryptionType.Aes256;

        public IDictionary<AccountError, string> Errors { get; }

        public Account() : this(string.Empty, string.Empty) { }
        private Account(string username, string password)
        {
            Username = username;
            Password = password;

            Errors = new Dictionary<AccountError, string>
            {
                //NotAuthenthicated by default
            };
        }
    }
}
