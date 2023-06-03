﻿using SNPM.MVVM.ViewModels;
using System;
using System.ComponentModel;

namespace SNPM.Core.Interfaces
{
    public interface IOption : INotifyPropertyChanged
    {
        public string Name { get; set; }

        public object? Value { get; set; }

        public ChangeableOption Option { get; set; }
    }
}
