using SNPM.Core.Api.Interfaces;
using SNPM.MVVM.Models.UiModels;
using SNPM.MVVM.Models.UiModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNPM.MVVM.Models.Interfaces
{
    public interface IAccountActivity
    {
        public List<LoginAttempt> LoginSucesses { get; set; }

        public List<LoginAttempt> LoginFails { get; set; }

        public DateTime CreationDate { get; set; }

        // TODO: (Przemek) Implement
        //public EncryptionType EncryptionType { get; set; }

        public bool Active2fa { get; set; }

        public bool MailConfirmed { get; set; }

        public DateTime LastPasswordChange { get; set; }

        public string Email { get; set; }

        public int AllEntriesCount { get; set; }
    }
}
