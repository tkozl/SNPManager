using System;
using System.Threading.Tasks;

namespace SNPM.Core.Api.Interfaces
{
    public interface IToken : IDisposable
    {
        delegate Task<DateTime> RefreshTokenDelegate(IToken token);

        string SessionToken { get; set; }

        DateTime ExpirationTime { get; set; }

        public RefreshTokenDelegate? RefreshTokenMethod { set; }
    }
}
