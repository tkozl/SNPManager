using System;

namespace SNPM.MVVM.Models.UiModels.Interfaces
{
    public interface ILoginAttempt
    {
        public string Ip { get; set; }

        public DateTime AttemptTime { get; set; }

        public bool IsSuccessful { get; set; }
    }
}
