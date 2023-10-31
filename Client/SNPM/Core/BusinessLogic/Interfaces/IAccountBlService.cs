using SNPM.Core.Api.Interfaces;
using SNPM.MVVM.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        Task<IAccountActivity> GetAccountActivity(string sessionToken);
    }
}
