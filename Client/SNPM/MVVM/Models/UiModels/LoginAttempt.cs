using Newtonsoft.Json;
using SNPM.Core.Converters;
using SNPM.MVVM.Models.UiModels.Interfaces;
using System;

namespace SNPM.MVVM.Models.UiModels
{
    public class LoginAttempt : ILoginAttempt
    {
        public LoginAttempt()
        {
        }

        //public LoginAttempt(string ip, DateTime attemptTime, bool isSuccessful)
        //{
        //    Ip = ip;
        //    AttemptTime = attemptTime;
        //    IsSuccessful = isSuccessful;
        //}

        [JsonProperty("ip")]
        public string Ip { get; set; }

        [JsonProperty("time")]
        [JsonConverter(typeof(UtcDateTimeConverter))]
        public DateTime AttemptTime { get; set; }

        public bool IsSuccessful { get; set; }
    }
}
