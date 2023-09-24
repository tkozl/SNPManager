using SNPM.Core.Api.Interfaces;
using System;
using System.Threading;
using static SNPM.Core.Api.Interfaces.IToken;

namespace SNPM.Core.Api
{
    public class Token : IToken
    {
        private static int UpdateInterval = 5;

        private Timer updateTimer;
        
        public string SessionToken { 
            get => authenthicationToken;
            
            set
            {
                ExpirationTime = DateTime.UtcNow + TimeSpan.FromMinutes(19);
                authenthicationToken = value;
            }
        }
        private string authenthicationToken;

        public DateTime ExpirationTime { get; set; }

        public RefreshTokenDelegate? RefreshTokenMethod { get; set; }

        //public Token(string SessionToken, DateTime ExpirationTime, RefreshTokenMethod refreshTokenMethod)
        //{
        //    authenthicationToken = string.Empty;

        //    this.SessionToken = SessionToken;
        //    this.ExpirationTime = ExpirationTime;
        //    this.RefreshTokenMethod = refreshTokenMethod;

        //    updateTimer = new Timer((e) => UpdateToken(), null, 0, UpdateInterval);
        //}

        public Token()
        {
            authenthicationToken = string.Empty;
            updateTimer = new Timer((e) => UpdateToken(), null, 0, UpdateInterval * 1000);
        }

        void UpdateToken()
        {
            if (DateTime.UtcNow.Subtract(ExpirationTime).TotalSeconds < 120 && RefreshTokenMethod != null)
            {
                RefreshTokenMethod(this);
            }
        }

        public void Dispose()
        {
            updateTimer.Dispose();
        }
    }
}
