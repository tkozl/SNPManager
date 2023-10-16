using SNPM.MVVM.Views.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNPM.MVVM.ViewModels.Interfaces
{
    internal interface IMainViewModel : IViewModel
    {
        Action CloseAction { get; set; }
        
        IDirectoryViewModel DirectoryTreeViewModel { get; }

        IntPtr MainWindowHandle { get; }

        void SubscribeToPreferenceUpdate(PreferenceHandler handler);
    }
}
