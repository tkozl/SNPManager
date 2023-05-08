using SNPM.Core.Interfaces;
using System;

namespace SNPM.Core
{
    public class Account : IAccount
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string SessionToken { get; set; } // TODO: (Przemek) Create a separate class for handling tokens
        public DateTime TokenExpirationDate { get; set; }
        public bool IsAuthenticated => SessionToken == string.Empty || DateTime.Now > TokenExpirationDate;

        public Account() : this(string.Empty, string.Empty, string.Empty) { }
        public Account(string username, string password) : this(username, password, string.Empty) { }

        public Account(string username, string password, string sessionToken)
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
        }

        public bool CheckIfCorrect()
        {
            return Username != null && Username.Length > 5 && Password != null && Password.Length > 5;
        }
    }
}
