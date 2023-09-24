using SNPM.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SNPM.Core.Options
{
    public class CheckBoxOption : IOption
    {
        public string Name { get; set;  }

        public object? Value
        {
            get => _value;
            set
            {
                if (value != null && value is bool boolValue)
                {
                    _value = boolValue;
                }
                else
                {
                    _value = false;
                }

                OnPropertyChanged();
            }
        }

        public ChangeableOption Option { get ; set; }

        public bool _value;

        public event PropertyChangedEventHandler? PropertyChanged;
        
        public CheckBoxOption(ChangeableOption name)
        {
            Name = name.ToString();
        }

        protected void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }
    }
}
