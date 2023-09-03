using SNPM.MVVM.Models.UiModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SNPM.MVVM.Models.UiModels
{
    internal class UiDirectory : MenuItem, IUiDirectory
    {
        public int Id { get; set; }

        public int ParentId { get; set; }
    }
}
