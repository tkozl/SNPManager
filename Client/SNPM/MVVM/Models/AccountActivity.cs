using Newtonsoft.Json;
using SNPM.Core.Converters;
using SNPM.MVVM.Models.Interfaces;
using SNPM.MVVM.Models.UiModels;
using SNPM.MVVM.Models.UiModels.Interfaces;
using System;
using System.Collections.Generic;

namespace SNPM.MVVM.Models
{
    internal class AccountActivity : IAccountActivity
    {
        private List<LoginAttempt> loginSucesses;
        private List<LoginAttempt> loginFails;

        public AccountActivity(
            List<LoginAttempt> loginSucesses,
            List<LoginAttempt> loginFails,
            DateTime creationDate,
            bool active2fa,
            bool mailConfirmed,
            DateTime lastPasswordChange,
            string email,
            int allEntriesCount)
        {
            LoginSucesses = loginSucesses;
            LoginFails = loginFails;
            CreationDate = creationDate;
            Active2fa = active2fa;
            MailConfirmed = mailConfirmed;
            LastPasswordChange = lastPasswordChange;
            Email = email;
            AllEntriesCount = allEntriesCount;
        }

        [JsonProperty("lastAccess")]
        public List<LoginAttempt> LoginSucesses
        {
            get => loginSucesses;
            set
            {
                value.ForEach(x => x.IsSuccessful = true);
                loginSucesses = value;
            }
        }

        [JsonProperty("lastLoginErrors")]
        public List<LoginAttempt> LoginFails
        {
            get => loginFails;
            set
            {
                value.ForEach(x => x.IsSuccessful = false);
                loginFails = value;
            }
        }

        [JsonProperty("creationDate")]
        [JsonConverter(typeof(UtcDateTimeConverter))]
        public DateTime CreationDate { get; set; }

        [JsonProperty("is2faActive")]
        public bool Active2fa { get; set; }

        [JsonProperty("isMailActive")]
        public bool MailConfirmed { get; set; }

        [JsonProperty("lastPasswordChange")]
        [JsonConverter(typeof(UtcDateTimeConverter))]
        public DateTime LastPasswordChange { get; set; }

        [JsonProperty("mail")]
        public string Email { get; set; }

        [JsonProperty("numberOfEntries")]
        public int AllEntriesCount { get; set; }
    }
}
