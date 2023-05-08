using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNPM.Core.Interfaces
{
    public interface IAccount
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string SessionToken { get; set; }

        public DateTime TokenExpirationDate { get; set; }

        public bool IsAuthenticated { get; }

        public bool CheckIfCorrect();
    }
}
