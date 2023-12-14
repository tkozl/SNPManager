using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNPM.MVVM.Models.UiModels.Interfaces
{
    public interface IUiParameter : INotifyPropertyChanged
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
}
