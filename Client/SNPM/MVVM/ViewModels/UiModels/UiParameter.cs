using SNPM.Core;
using SNPM.MVVM.Models.UiModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNPM.MVVM.Models.UiModels
{
    internal class UiParameter : ObservableObject, IUiParameter
    {
        private string name;
        private string value;

        public UiParameter(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Value
        {
            get => value;
            set
            {
                this.value = value;
                OnPropertyChanged(nameof(Value));
            }
        }
    }
}
