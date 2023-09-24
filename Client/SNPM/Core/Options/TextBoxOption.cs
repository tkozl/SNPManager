using SNPM.MVVM.ViewModels;
using System;
using System.ComponentModel;

namespace SNPM.Core.Options
{
    public class TextBoxOption : IOption
    {
        public string Name { get; set; }

        public object? Value {
            get => _value;
            set
            {
                if (value == null)
                {
                    _value = String.Empty;
                }
                else
                {
                    _value = (string)value;
                }

                OnPropertyChanged();
            }
        }

        public ChangeableOption Option { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        private string _value;

        public TextBoxOption(ChangeableOption Option)
        {
            this.Name = Option.ToString();
            this._value = String.Empty;
            this.Option = Option;
        }

        protected void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }
    }
}
