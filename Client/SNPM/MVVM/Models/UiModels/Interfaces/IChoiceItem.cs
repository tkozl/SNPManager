using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNPM.MVVM.Models.Interfaces
{
    public interface IChoiceItem
    {
        object Item { get; }

        int Id { get; }

        string DisplayName { get; set; }
    }
}
