using Microsoft.Extensions.DependencyInjection;
using SNPM.MVVM.Models.UiModels;
using SNPM.MVVM.ViewModels.Interfaces;
using SNPM.MVVM.Views.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNPM.MVVM.ViewModels
{
    class DirectoryViewModel : IDirectoryViewModel
    {
        public IDirectoryView View { get; set; }

        public ObservableCollection<Directory> Directories { get; }

        public DirectoryViewModel()
        {
            Directories = new ObservableCollection<Directory>();
        }

        public void ShowView()
        {
        }

        public void HideView()
        {
        }
    }
}
