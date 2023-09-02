using SNPM.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNPM.Core.BusinessLogic
{
    public interface IAccountBlService
    {
        Task CreateAccount(IAccount account);

        Task Login(IAccount account);
    }
}
