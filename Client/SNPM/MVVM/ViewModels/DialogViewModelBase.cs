using SNPM.Core;
using SNPM.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNPM.MVVM.ViewModels
{
    public abstract class DialogViewModelBase<T> : IDialogViewModel
    {
        public DialogViewModelBase()
        {

        }

        public IEnumerable<ObservableObject> dialogFields;
    }
}
