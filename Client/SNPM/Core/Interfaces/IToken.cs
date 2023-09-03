using System;

namespace SNPM.Core.Interfaces
{
    public interface IToken : IDisposable
    {
        delegate DateTime RefreshTokenDelegate(IToken token);

        string SessionToken { get; set; }

        DateTime ExpirationTime { get; set; }

        public RefreshTokenDelegate? RefreshTokenMethod { set; }
    }
}
