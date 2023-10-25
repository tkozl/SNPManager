using SNPM.Core.Api.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using static SNPM.Core.Api.Interfaces.IToken;

namespace SNPM.Core.Api
{
    public class Token : IToken
    {
        private static int UpdateInterval = 5;
        private bool tokenLock;

        private Timer updateTimer;
        
        public string SessionToken {
            get => authenthicationToken;

            set => authenthicationToken = value;
        }
        private string authenthicationToken;

        public DateTime ExpirationTime { get; set; }

        public RefreshTokenDelegate? RefreshTokenMethod { get; set; }

        public Token()
        {
            authenthicationToken = string.Empty;
            updateTimer = new Timer(async (e) => await UpdateToken(), null, 0, UpdateInterval * 1000);

            tokenLock = false;
        }

        async Task UpdateToken()
        {
            if (ExpirationTime.Subtract(DateTime.UtcNow).TotalSeconds < 30 && RefreshTokenMethod != null && !tokenLock)
            {
                tokenLock = true;

                ExpirationTime = await RefreshTokenMethod(this);

                tokenLock = false;
            }
        }

        public void Dispose()
        {
            updateTimer.Dispose();
        }
    }
}
