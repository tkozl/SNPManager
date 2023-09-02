﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNPM.MVVM.ViewModels.Interfaces
{
    internal interface IMainViewModel : IViewModel
    {
        public Action CloseAction { get; set; }

        public void SubscribeToPreferenceUpdate(PreferenceHandler handler);
    }
}
