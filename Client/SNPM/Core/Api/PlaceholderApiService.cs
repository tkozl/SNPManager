using SNPM.Core.Interfaces.Api;
using System;
using System.Threading.Tasks;

namespace SNPM.Core.Api
{
    public class PlaceholderApiService : IApiService
    {
        public PlaceholderApiService()
        {

        }

        public Func<string, Task<bool>> GetRemoteVerifier()
        {
            return VerifyPasswordRemotely;
        }

        private async Task<bool> VerifyPasswordRemotely(string passwordHash)
        {
            await Task.Delay(1);
            return true;
        }
    }
}
