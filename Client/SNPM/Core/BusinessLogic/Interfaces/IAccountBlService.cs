using SNPM.Core.Api.Interfaces;
using SNPM.MVVM.Models.Interfaces;
using System;
using System.Threading.Tasks;

namespace SNPM.Core.BusinessLogic.Interfaces
{
    public interface IAccountBlService
    {
        event EventHandler AccountLoggedIn;

        IToken? ActiveToken { get; }
        IAccountActivity AccountActivity { get; set; }

        Task CreateAccount(IAccount account);

        Task Login(IAccount account);

        Task<bool> AuthorizeSecondFactor(string code);

        Task<IAccountActivity> GetAccountActivity(string sessionToken);

        Task<string> Toggle2Fa();
        Task<PasswordQuality> VerifyPassword(string password);
    }
}
