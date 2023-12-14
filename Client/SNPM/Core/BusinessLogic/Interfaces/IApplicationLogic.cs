using SNPM.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNPM.Core.BusinessLogic.Interfaces
{
    public interface IApplicationLogic
    {
        public delegate void OptionChanged(ChangeableOption option, object value);

        Action OnExit { get; }

        public event OptionChanged OnOptionChange;
    }
}
