using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNPM.MVVM.ViewModels.Interfaces
{
    internal interface IMainViewModel
    {
        public Action CloseAction { get; set; }
    }
}
