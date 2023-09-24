using SNPM.MVVM.Models.Interfaces;
using System;
using System.Threading.Tasks;

namespace SNPM.Core.BusinessLogic.Interfaces
{
    public interface IPasswordVerifier
    {
        public Task<PasswordQuality> VerifyPassword(string password);

        public IPasswordPolicy PasswordPolicy { get; set; }
    }
}
