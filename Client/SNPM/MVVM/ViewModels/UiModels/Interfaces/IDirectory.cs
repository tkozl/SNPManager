using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SNPM.MVVM.Models.UiModels.Interfaces
{
    public interface IDirectory
    {
        string Name { get; set; }

        int Id { get; }

        int ParentId { get; }
    }
}
